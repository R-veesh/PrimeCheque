using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface ITemplateService
    {
        Task<List<BankTemplate>> GetAllTemplatesAsync();
        Task<BankTemplate?> GetTemplateByIdAsync(Guid id);
        Task<BankTemplate?> GetTemplateForBankAsync(Guid bankId);
        Task<BankTemplate> SaveTemplateAsync(BankTemplate template);
        Task DeleteTemplateAsync(Guid id);
        Task<int> SyncTemplatesFromCloudAsync();
    }
}
