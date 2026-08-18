using System;
using System.Threading.Tasks;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class LicenceService : ILicenceService
    {
        private readonly IApiIntegrationService _apiService;
        private static string _savedLicenceKey = "PRIME-CHEQUE-TRIAL-2026";
        private static bool _isActivated = false;

        public LicenceService(IApiIntegrationService apiService)
        {
            _apiService = apiService;
        }

        public async Task<LicenceInfo> GetLicenceInfoAsync()
        {
            var apiStatus = await _apiService.GetLicenceStatusAsync();
            if (apiStatus != null)
            {
                _isActivated = apiStatus.Status.ToLower() == "activated";
                return new LicenceInfo
                {
                    LicenceKey = _savedLicenceKey,
                    Status = $"Cloud Verified: {apiStatus.Status} ({apiStatus.Edition})",
                    IsValid = _isActivated,
                    RemainingGraceDays = apiStatus.ExpiresAt.HasValue ? (apiStatus.ExpiresAt.Value - DateTime.UtcNow).Days : 365,
                    MachineId = Environment.MachineName
                };
            }

            // Offline fallback
            var info = new LicenceInfo
            {
                LicenceKey = _savedLicenceKey,
                Status = _isActivated ? "Activated (Offline Cache)" : "Trial / Offline Grace Period",
                IsValid = true,
                RemainingGraceDays = _isActivated ? 365 : 30,
                MachineId = Environment.MachineName
            };

            return info;
        }

        public async Task<bool> ActivateLicenceAsync(string licenceKey)
        {
            if (!string.IsNullOrWhiteSpace(licenceKey) && licenceKey.Length >= 8)
            {
                // Try to authenticate with the new key
                var authSuccess = await _apiService.AuthenticateAsync(licenceKey.Trim(), Environment.MachineName);
                if (authSuccess)
                {
                    var activationResult = await _apiService.ActivateLicenceAsync(licenceKey.Trim(), Environment.MachineName, Environment.MachineName);
                    if (activationResult != null && activationResult.Status.ToLower() == "activated")
                    {
                        _savedLicenceKey = licenceKey.Trim();
                        _isActivated = true;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
