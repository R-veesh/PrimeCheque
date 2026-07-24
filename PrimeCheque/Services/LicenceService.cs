using System;
using System.Threading.Tasks;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class LicenceService : ILicenceService
    {
        private static string _savedLicenceKey = "PRIME-CHEQUE-TRIAL-2026";
        private static bool _isActivated = false;

        public Task<LicenceInfo> GetLicenceInfoAsync()
        {
            var info = new LicenceInfo
            {
                LicenceKey = _savedLicenceKey,
                Status = _isActivated ? "Activated (Enterprise)" : "Trial / Offline Grace Period",
                IsValid = true,
                RemainingGraceDays = _isActivated ? 365 : 30,
                MachineId = Environment.MachineName
            };

            return Task.FromResult(info);
        }

        public Task<bool> ActivateLicenceAsync(string licenceKey)
        {
            if (!string.IsNullOrWhiteSpace(licenceKey) && licenceKey.Length >= 8)
            {
                _savedLicenceKey = licenceKey.Trim();
                _isActivated = true;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
