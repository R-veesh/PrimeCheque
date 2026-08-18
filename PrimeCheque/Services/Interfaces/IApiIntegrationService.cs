using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IApiIntegrationService
    {
        Task<bool> AuthenticateAsync(string licenceKey, string machineId);
        Task<LicenceStatusDto?> ActivateLicenceAsync(string licenceKey, string machineId, string machineName);
        Task<LicenceStatusDto?> GetLicenceStatusAsync();
        Task<List<BankTemplateDto>?> FetchTemplatesAsync();
        Task<BackupResponseDto?> UploadBackupAsync(string filePath, string machineId);
        Task<bool> IsHealthyAsync();
    }
}
