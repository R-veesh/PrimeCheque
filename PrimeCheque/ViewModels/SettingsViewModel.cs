using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IBackupService _backupService;

        private readonly ILicenceService _licenceService;

        [ObservableProperty]
        private string _amountPrefix = "Sri Lanka Rupees";

        [ObservableProperty]
        private string _amountSuffix = "Only";

        [ObservableProperty]
        private string _centsWord = "Cents";

        [ObservableProperty]
        private bool _useAnd = true;

        [ObservableProperty]
        private string _backupStatusMessage = string.Empty;

        [ObservableProperty]
        private string _licenceKey = string.Empty;

        [ObservableProperty]
        private string _licenceStatus = "Checking...";

        [ObservableProperty]
        private int _remainingGraceDays = 30;

        public SettingsViewModel(IBackupService backupService, ILicenceService licenceService)
        {
            _backupService = backupService;
            _licenceService = licenceService;
        }

        public async Task LoadSettingsAsync()
        {
            var info = await _licenceService.GetLicenceInfoAsync();
            LicenceKey = info.LicenceKey;
            LicenceStatus = info.Status;
            RemainingGraceDays = info.RemainingGraceDays;
        }

        [RelayCommand]
        private async Task ActivateLicenceAsync()
        {
            var success = await _licenceService.ActivateLicenceAsync(LicenceKey);
            if (success)
            {
                var info = await _licenceService.GetLicenceInfoAsync();
                LicenceStatus = info.Status;
                RemainingGraceDays = info.RemainingGraceDays;
                BackupStatusMessage = "Licence key activated successfully!";
            }
            else
            {
                BackupStatusMessage = "Invalid licence key.";
            }
        }

        [RelayCommand]
        private async Task CreateBackupAsync()
        {
            try
            {
                var folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "PrimeChequeBackups");
                var path = await _backupService.CreateBackupAsync(folder);
                BackupStatusMessage = $"Backup created successfully at: {path}";
            }
            catch (Exception ex)
            {
                BackupStatusMessage = $"Backup failed: {ex.Message}";
            }
        }
    }
}
