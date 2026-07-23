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
    public class ReportService : IReportService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public ReportService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Cheque>> GetChequeRegisterAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var start = DateOnly.FromDateTime(startDate);
            var end = DateOnly.FromDateTime(endDate);

            return await _dbContext.Cheques
                .Include(c => c.Company)
                .Include(c => c.ChequeBook)
                    .ThenInclude(cb => cb.Bank)
                .Where(c => c.CompanyId == companyId && c.ChequeDate >= start && c.ChequeDate <= end)
                .OrderBy(c => c.ChequeDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Cheque>> GetPostDatedChequesAsync(Guid companyId, DateTime minDate)
        {
            var today = DateOnly.FromDateTime(minDate);

            return await _dbContext.Cheques
                .Include(c => c.Company)
                .Include(c => c.ChequeBook)
                    .ThenInclude(cb => cb.Bank)
                .Where(c => c.CompanyId == companyId && c.ChequeDate > today && c.Status != ChequeStatus.Void)
                .OrderBy(c => c.ChequeDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Cheque>> GetVoidChequesAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var start = DateOnly.FromDateTime(startDate);
            var end = DateOnly.FromDateTime(endDate);

            return await _dbContext.Cheques
                .Include(c => c.Company)
                .Include(c => c.ChequeBook)
                    .ThenInclude(cb => cb.Bank)
                .Where(c => c.CompanyId == companyId && c.Status == ChequeStatus.Void && c.ChequeDate >= start && c.ChequeDate <= end)
                .OrderBy(c => c.ChequeDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
