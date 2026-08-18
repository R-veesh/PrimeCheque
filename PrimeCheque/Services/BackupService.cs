using System;
using System.IO;
using System.Threading.Tasks;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class BackupService : IBackupService
    {
        private readonly IApiIntegrationService _apiService;

        public BackupService(IApiIntegrationService apiService)
        {
            _apiService = apiService;
        }

        public Task<string> CreateBackupAsync(string destinationDirectory)
        {
            var sourceDbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrimeCheque", "primecheque.db");
            if (!File.Exists(sourceDbPath))
            {
                throw new FileNotFoundException("Database file not found to backup.", sourceDbPath);
            }

            Directory.CreateDirectory(destinationDirectory);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"PrimeCheque_Backup_{timestamp}.db";
            var destPath = Path.Combine(destinationDirectory, backupFileName);

            File.Copy(sourceDbPath, destPath, overwrite: true);
            return Task.FromResult(destPath);
        }

        public Task<bool> RestoreBackupAsync(string backupFilePath)
        {
            if (!File.Exists(backupFilePath))
            {
                return Task.FromResult(false);
            }

            var destDbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrimeCheque", "primecheque.db");
            File.Copy(backupFilePath, destDbPath, overwrite: true);
            return Task.FromResult(true);
        }

        public async Task<bool> BackupToCloudAsync(string sourceDbPath)
        {
            if (!File.Exists(sourceDbPath)) return false;

            var result = await _apiService.UploadBackupAsync(sourceDbPath, Environment.MachineName);
            return result != null;
        }
    }
}
