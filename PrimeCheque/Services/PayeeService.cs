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
    public class PayeeService : IPayeeService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public PayeeService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Payee>> GetPayeesByCompanyAsync(Guid companyId)
        {
            return await _dbContext.Payees
                .Where(p => p.CompanyId == companyId)
                .OrderByDescending(p => p.IsFavourite)
                .ThenBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Payee>> SearchPayeesAsync(Guid companyId, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetPayeesByCompanyAsync(companyId);

            return await _dbContext.Payees
                .Where(p => p.CompanyId == companyId && (EF.Functions.Like(p.Name, $"%{query}%") || (p.NickName != null && EF.Functions.Like(p.NickName, $"%{query}%"))))
                .OrderByDescending(p => p.IsFavourite)
                .ThenBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Payee> AddOrUpdatePayeeAsync(Payee payee)
        {
            var existing = await _dbContext.Payees.FirstOrDefaultAsync(p => p.Id == payee.Id);
            if (existing == null)
            {
                payee.CreatedAt = DateTime.UtcNow;
                _dbContext.Payees.Add(payee);
            }
            else
            {
                existing.Name = payee.Name;
                existing.NickName = payee.NickName;
                existing.DefaultMemo = payee.DefaultMemo;
                existing.LastAmount = payee.LastAmount;
                existing.IsFavourite = payee.IsFavourite;
                _dbContext.Payees.Update(existing);
            }
            await _dbContext.SaveChangesAsync();
            return payee;
        }

        public async Task DeletePayeeAsync(Guid id)
        {
            var payee = await _dbContext.Payees.FindAsync(id);
            if (payee != null)
            {
                _dbContext.Payees.Remove(payee);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
