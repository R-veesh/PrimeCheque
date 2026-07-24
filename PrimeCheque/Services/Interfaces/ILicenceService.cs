using System.Threading.Tasks;

namespace PrimeCheque.Services.Interfaces
{
    public class LicenceInfo
    {
        public string LicenceKey { get; set; } = string.Empty;
        public string Status { get; set; } = "Trial";
        public bool IsValid { get; set; } = true;
        public int RemainingGraceDays { get; set; } = 30;
        public string MachineId { get; set; } = string.Empty;
    }

    public interface ILicenceService
    {
        Task<LicenceInfo> GetLicenceInfoAsync();
        Task<bool> ActivateLicenceAsync(string licenceKey);
    }
}
