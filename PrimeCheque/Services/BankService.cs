using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class BankService : IBankService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public BankService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Bank>> GetAllBanksAsync()
        {
            return await _dbContext.Banks.AsNoTracking().ToListAsync();
        }

        public async Task<Bank?> GetBankByIdAsync(Guid id)
        {
            return await _dbContext.Banks.FindAsync(id);
        }

        public async Task<Bank> CreateBankAsync(Bank bank)
        {
            bank.CreatedAt = DateTime.UtcNow;
            _dbContext.Banks.Add(bank);
            await _dbContext.SaveChangesAsync();
            return bank;
        }

        public async Task<Bank> UpdateBankAsync(Bank bank)
        {
            _dbContext.Banks.Update(bank);
            await _dbContext.SaveChangesAsync();
            return bank;
        }

        public async Task DeleteBankAsync(Guid id)
        {
            var bank = await _dbContext.Banks.FindAsync(id);
            if (bank != null)
            {
                _dbContext.Banks.Remove(bank);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
