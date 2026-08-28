using System.Threading.Tasks;

namespace PrimeCheque.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<string?> GetSettingAsync(string key, string? defaultValue = null);
        Task SetSettingAsync(string key, string value);
        Task<AmountToWordsOptions> GetAmountToWordsOptionsAsync();
        Task SaveAmountToWordsOptionsAsync(AmountToWordsOptions options);
    }
}
