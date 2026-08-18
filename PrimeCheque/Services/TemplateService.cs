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
    public class TemplateService : ITemplateService
    {
        private readonly PrimeChequeDbContext _dbContext;
        private readonly IApiIntegrationService _apiService;

        public TemplateService(PrimeChequeDbContext dbContext, IApiIntegrationService apiService)
        {
            _dbContext = dbContext;
            _apiService = apiService;
        }

        public async Task<List<BankTemplate>> GetAllTemplatesAsync()
        {
            return await _dbContext.BankTemplates.Include(t => t.Bank).AsNoTracking().ToListAsync();
        }

        public async Task<BankTemplate?> GetTemplateByIdAsync(Guid id)
        {
            return await _dbContext.BankTemplates.Include(t => t.Bank).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<BankTemplate?> GetTemplateForBankAsync(Guid bankId)
        {
            return await _dbContext.BankTemplates
                .Where(t => t.BankId == bankId)
                .OrderByDescending(t => t.IsDefault)
                .FirstOrDefaultAsync();
        }

        public async Task<BankTemplate> SaveTemplateAsync(BankTemplate template)
        {
            var existing = await _dbContext.BankTemplates.FindAsync(template.Id);
            if (existing == null)
            {
                template.CreatedAt = DateTime.UtcNow;
                _dbContext.BankTemplates.Add(template);
            }
            else
            {
                existing.BankName = template.BankName;
                existing.SeriesName = template.SeriesName;
                existing.TemplateConfig = template.TemplateConfig;
                existing.ChequeWidthMm = template.ChequeWidthMm;
                existing.ChequeHeightMm = template.ChequeHeightMm;
                existing.IsDefault = template.IsDefault;
                existing.BankId = template.BankId;
                _dbContext.BankTemplates.Update(existing);
            }
            await _dbContext.SaveChangesAsync();
            return template;
        }

        public async Task DeleteTemplateAsync(Guid id)
        {
            var template = await _dbContext.BankTemplates.FindAsync(id);
            if (template != null)
            {
                _dbContext.BankTemplates.Remove(template);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        public async Task<int> SyncTemplatesFromCloudAsync()
        {
            var templatesDto = await _apiService.FetchTemplatesAsync();
            if (templatesDto == null || !templatesDto.Any()) return 0;
            
            int addedOrUpdated = 0;
            
            foreach (var dto in templatesDto)
            {
                // Find existing bank
                var bank = await _dbContext.Banks.FirstOrDefaultAsync(b => b.Name.ToLower() == dto.BankName.ToLower());
                Guid? bankId = bank?.Id;
                
                var existingTemplate = await _dbContext.BankTemplates
                    .FirstOrDefaultAsync(t => t.BankName == dto.BankName && t.SeriesName == dto.SeriesName);
                    
                if (existingTemplate != null)
                {
                    existingTemplate.TemplateConfig = dto.TemplateConfig;
                    existingTemplate.ChequeWidthMm = dto.Dimensions?.Width ?? existingTemplate.ChequeWidthMm;
                    existingTemplate.ChequeHeightMm = dto.Dimensions?.Height ?? existingTemplate.ChequeHeightMm;
                    existingTemplate.TemplateImagePath = dto.TemplateImagePath ?? existingTemplate.TemplateImagePath;
                    _dbContext.BankTemplates.Update(existingTemplate);
                    addedOrUpdated++;
                }
                else
                {
                    var newTemplate = new BankTemplate
                    {
                        Id = Guid.NewGuid(),
                        BankName = dto.BankName,
                        SeriesName = dto.SeriesName,
                        TemplateConfig = dto.TemplateConfig,
                        ChequeWidthMm = dto.Dimensions?.Width ?? 200,
                        ChequeHeightMm = dto.Dimensions?.Height ?? 90,
                        TemplateImagePath = dto.TemplateImagePath,
                        IsDefault = true,
                        BankId = bankId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _dbContext.BankTemplates.Add(newTemplate);
                    addedOrUpdated++;
                }
            }
            
            if (addedOrUpdated > 0)
            {
                await _dbContext.SaveChangesAsync();
            }
            
            return addedOrUpdated;
        }
    }
}
