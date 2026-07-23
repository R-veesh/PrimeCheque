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

        public SettingsViewModel(IBackupService backupService)
        {
            _backupService = backupService;
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
