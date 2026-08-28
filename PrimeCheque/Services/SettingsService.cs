using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public SettingsService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string?> GetSettingAsync(string key, string? defaultValue = null)
        {
            var setting = await _dbContext.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
            return setting?.Value ?? defaultValue;
        }

        public async Task SetSettingAsync(string key, string value)
        {
            var setting = await _dbContext.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting != null)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _dbContext.AppSettings.Add(new AppSettings
                {
                    Key = key,
                    Value = value,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AmountToWordsOptions> GetAmountToWordsOptionsAsync()
        {
            var prefix = await GetSettingAsync("AmountPrefix", "Sri Lanka Rupees");
            var suffix = await GetSettingAsync("AmountSuffix", "Only");
            var centsWord = await GetSettingAsync("CentsWord", "Cents");
            var useAndStr = await GetSettingAsync("UseAnd", "True");
            bool.TryParse(useAndStr, out var useAnd);

            var options = new AmountToWordsOptions
            {
                Prefix = prefix ?? "Sri Lanka Rupees",
                Suffix = suffix ?? "Only",
                CentsWord = centsWord ?? "Cents",
                UseAnd = useAnd
            };

            AmountToWordsService.DefaultOptions = options;
            return options;
        }

        public async Task SaveAmountToWordsOptionsAsync(AmountToWordsOptions options)
        {
            await SetSettingAsync("AmountPrefix", options.Prefix ?? string.Empty);
            await SetSettingAsync("AmountSuffix", options.Suffix ?? string.Empty);
            await SetSettingAsync("CentsWord", options.CentsWord ?? string.Empty);
            await SetSettingAsync("UseAnd", options.UseAnd.ToString());

            AmountToWordsService.DefaultOptions = options;
        }
    }
}
