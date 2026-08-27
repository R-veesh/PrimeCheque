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

        public TemplateService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
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
                existing.TemplateImagePath = template.TemplateImagePath;

                // Force EF to detect changes — needed when the tracked entity
                // is the same reference as the incoming template object
                _dbContext.Entry(existing).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
    }
}
