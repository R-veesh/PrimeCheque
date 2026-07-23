using System.Threading.Tasks;

namespace PrimeCheque.Services.Interfaces
{
    public interface IBackupService
    {
        Task<string> CreateBackupAsync(string destinationDirectory);
        Task<bool> RestoreBackupAsync(string backupFilePath);
    }
}
