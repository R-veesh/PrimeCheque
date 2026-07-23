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
    public class ChequeBookService : IChequeBookService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public ChequeBookService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ChequeBook>> GetChequeBooksByCompanyAsync(Guid companyId)
        {
            return await _dbContext.ChequeBooks
                .Include(cb => cb.Bank)
                .Include(cb => cb.Company)
                .Where(cb => cb.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ChequeBook?> GetChequeBookByIdAsync(Guid id)
        {
            return await _dbContext.ChequeBooks
                .Include(cb => cb.Bank)
                .Include(cb => cb.Company)
                .FirstOrDefaultAsync(cb => cb.Id == id);
        }

        public async Task<ChequeBook> AddChequeBookAsync(ChequeBook chequeBook)
        {
            chequeBook.CreatedAt = DateTime.UtcNow;
            if (chequeBook.CurrentChequeNo == 0)
            {
                chequeBook.CurrentChequeNo = chequeBook.StartChequeNo;
            }
            _dbContext.ChequeBooks.Add(chequeBook);
            await _dbContext.SaveChangesAsync();
            return chequeBook;
        }

        public async Task<ChequeBook> UpdateChequeBookAsync(ChequeBook chequeBook)
        {
            _dbContext.ChequeBooks.Update(chequeBook);
            await _dbContext.SaveChangesAsync();
            return chequeBook;
        }

        public async Task<int> GetNextChequeNumberAsync(Guid chequeBookId)
        {
            var book = await _dbContext.ChequeBooks.FindAsync(chequeBookId);
            if (book == null)
                throw new InvalidOperationException("Cheque book not found.");

            if (book.Status != ChequeBookStatus.Active)
                throw new InvalidOperationException("Cheque book is not active.");

            if (book.CurrentChequeNo > book.EndChequeNo)
            {
                book.Status = ChequeBookStatus.Exhausted;
                await _dbContext.SaveChangesAsync();
                throw new InvalidOperationException("Cheque book has been exhausted.");
            }

            return book.CurrentChequeNo;
        }
    }
}
