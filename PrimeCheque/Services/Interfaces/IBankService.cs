using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IBankService
    {
        Task<List<Bank>> GetAllBanksAsync();
        Task<Bank?> GetBankByIdAsync(Guid id);
        Task<Bank> CreateBankAsync(Bank bank);
        Task<Bank> UpdateBankAsync(Bank bank);
        Task DeleteBankAsync(Guid id);
    }
}
