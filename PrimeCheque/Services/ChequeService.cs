using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class ChequeService : IChequeService
    {
        private readonly PrimeChequeDbContext _dbContext;
        private readonly IAuditService _auditService;

        public ChequeService(PrimeChequeDbContext dbContext, IAuditService auditService)
        {
            _dbContext = dbContext;
            _auditService = auditService;
        }

        public async Task<Cheque?> GetChequeByIdAsync(Guid id)
        {
            return await _dbContext.Cheques
                .Include(c => c.Company)
                .Include(c => c.ChequeBook)
                    .ThenInclude(cb => cb.Bank)
                .Include(c => c.AuditLogs)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Cheque>> GetChequesAsync(Guid? companyId = null, ChequeStatus? status = null, string? searchQuery = null)
        {
            var query = _dbContext.Cheques
                .Include(c => c.Company)
                .Include(c => c.ChequeBook)
                    .ThenInclude(cb => cb.Bank)
                .AsQueryable();

            if (companyId.HasValue)
            {
                query = query.Where(c => c.CompanyId == companyId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(c => c.PayeeName.Contains(searchQuery) ||
                                         c.ChequeNumber.ToString().Contains(searchQuery) ||
                                         (c.Memo != null && c.Memo.Contains(searchQuery)));
            }

            return await query.OrderByDescending(c => c.CreatedAt).AsNoTracking().ToListAsync();
        }

        public async Task<Cheque> CreateChequeAsync(Cheque cheque, string user)
        {
            var chequeBook = await _dbContext.ChequeBooks.FindAsync(cheque.ChequeBookId);
            if (chequeBook == null)
                throw new InvalidOperationException("Associated ChequeBook not found.");

            cheque.CreatedBy = user;
            cheque.CreatedAt = DateTime.UtcNow;

            // Increment current cheque number in cheque book
            chequeBook.CurrentChequeNo++;
            if (chequeBook.CurrentChequeNo > chequeBook.EndChequeNo)
            {
                chequeBook.Status = ChequeBookStatus.Exhausted;
            }

            _dbContext.Cheques.Add(cheque);
            await _dbContext.SaveChangesAsync();

            await _auditService.LogEventAsync(cheque.Id, "Created", user, null, cheque);

            return cheque;
        }

        public async Task<Cheque> UpdateChequeAsync(Cheque cheque, string user)
        {
            var existing = await _dbContext.Cheques.FindAsync(cheque.Id);
            if (existing == null)
                throw new InvalidOperationException("Cheque not found.");

            if (existing.Status == ChequeStatus.Printed || existing.Status == ChequeStatus.Void)
                throw new InvalidOperationException("Printed or Void cheques cannot be updated.");

            var beforeState = new { existing.PayeeName, existing.Amount, existing.ChequeDate, existing.CrossingType, existing.Status };

            existing.PayeeName = cheque.PayeeName;
            existing.Amount = cheque.Amount;
            existing.AmountInWords = cheque.AmountInWords;
            existing.ChequeDate = cheque.ChequeDate;
            existing.Memo = cheque.Memo;
            existing.CrossingType = cheque.CrossingType;
            existing.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            await _auditService.LogEventAsync(cheque.Id, "Updated", user, beforeState, existing);

            return existing;
        }

        public async Task<bool> SubmitForApprovalAsync(Guid chequeId, string user)
        {
            var cheque = await _dbContext.Cheques.FindAsync(chequeId);
            if (cheque == null) return false;

            cheque.Status = ChequeStatus.PendingApproval;
            cheque.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            await _auditService.LogEventAsync(chequeId, "SubmittedForApproval", user);
            return true;
        }

        public async Task<bool> ApproveChequeAsync(Guid chequeId, string approver)
        {
            var cheque = await _dbContext.Cheques.FindAsync(chequeId);
            if (cheque == null) return false;

            cheque.Status = ChequeStatus.Approved;
            cheque.ApprovedBy = approver;
            cheque.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            await _auditService.LogEventAsync(chequeId, "Approved", approver);
            return true;
        }

        public async Task<bool> RejectChequeAsync(Guid chequeId, string rejecter, string reason)
        {
            var cheque = await _dbContext.Cheques.FindAsync(chequeId);
            if (cheque == null) return false;

            cheque.Status = ChequeStatus.Rejected;
            cheque.RejectionReason = reason;
            cheque.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            await _auditService.LogEventAsync(chequeId, "Rejected", rejecter, null, new { reason });
            return true;
        }

        public async Task<bool> VoidChequeAsync(Guid chequeId, string user, string reason)
        {
            var cheque = await _dbContext.Cheques.FindAsync(chequeId);
            if (cheque == null) return false;

            cheque.Status = ChequeStatus.Void;
            cheque.Memo = (cheque.Memo + $" [VOID: {reason}]").Trim();
            cheque.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            await _auditService.LogEventAsync(chequeId, "Voided", user, null, new { Reason = reason });
            return true;
        }

        public async Task<bool> MarkAsPrintedAsync(Guid chequeId, string user, string pdfPath)
        {
            var cheque = await _dbContext.Cheques.FindAsync(chequeId);
            if (cheque == null) return false;

            cheque.Status = ChequeStatus.Printed;
            cheque.PrintedAt = DateTime.UtcNow;
            cheque.PdfPath = pdfPath;
            cheque.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            await _auditService.LogEventAsync(chequeId, "Printed", user, null, new { PdfPath = pdfPath });
            return true;
        }

        public async Task<bool> IsDuplicateChequeAsync(Guid companyId, string payeeName, decimal amount)
        {
            var cutoff = DateTime.UtcNow.AddHours(-24);
            return await _dbContext.Cheques.AnyAsync(c =>
                c.CompanyId == companyId &&
                c.PayeeName.ToLower() == payeeName.Trim().ToLower() &&
                c.Amount == amount &&
                c.CreatedAt >= cutoff &&
                c.Status != ChequeStatus.Void);
        }

        public async Task<List<int>> GetMissingSequenceNumbersAsync(Guid chequeBookId)
        {
            var book = await _dbContext.ChequeBooks.FindAsync(chequeBookId);
            if (book == null) return new List<int>();

            var existingNumbers = await _dbContext.Cheques
                .Where(c => c.ChequeBookId == chequeBookId)
                .Select(c => c.ChequeNumber)
                .ToListAsync();

            var missing = new List<int>();
            int maxNo = Math.Min(book.CurrentChequeNo - 1, book.EndChequeNo);

            for (int i = book.StartChequeNo; i <= maxNo; i++)
            {
                if (!existingNumbers.Contains(i))
                {
                    missing.Add(i);
                }
            }

            return missing;
        }
    }
}
