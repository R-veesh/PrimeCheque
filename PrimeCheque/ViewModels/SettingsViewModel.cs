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
        private readonly ISettingsService _settingsService;
        private readonly IAmountToWordsService _amountToWordsService;

        [ObservableProperty]
        private string _amountPrefix = "Sri Lanka Rupees";

        [ObservableProperty]
        private string _amountSuffix = "Only";

        [ObservableProperty]
        private string _centsWord = "Cents";

        [ObservableProperty]
        private bool _useAnd = true;

        [ObservableProperty]
        private string _saveStatusMessage = string.Empty;

        [ObservableProperty]
        private bool _isSaveSuccess;

        [ObservableProperty]
        private string _backupStatusMessage = string.Empty;

        [ObservableProperty]
        private string _licenceKey = string.Empty;

        [ObservableProperty]
        private string _licenceStatus = "Checking...";

        [ObservableProperty]
        private int _remainingGraceDays = 30;

        public string AmountPreview
        {
            get
            {
                try
                {
                    var options = new AmountToWordsOptions
                    {
                        Prefix = AmountPrefix?.Trim() ?? string.Empty,
                        Suffix = AmountSuffix?.Trim() ?? string.Empty,
                        CentsWord = CentsWord?.Trim() ?? string.Empty,
                        UseAnd = UseAnd
                    };
                    return _amountToWordsService.Convert(75250.50m, options);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        partial void OnAmountPrefixChanged(string value) => OnPropertyChanged(nameof(AmountPreview));
        partial void OnAmountSuffixChanged(string value) => OnPropertyChanged(nameof(AmountPreview));
        partial void OnCentsWordChanged(string value) => OnPropertyChanged(nameof(AmountPreview));
        partial void OnUseAndChanged(bool value) => OnPropertyChanged(nameof(AmountPreview));

        public SettingsViewModel(
            IBackupService backupService,
            ILicenceService licenceService,
            ISettingsService settingsService,
            IAmountToWordsService amountToWordsService)
        {
            _backupService = backupService;
            _licenceService = licenceService;
            _settingsService = settingsService;
            _amountToWordsService = amountToWordsService;
        }

        public async Task LoadSettingsAsync()
        {
            try
            {
                var options = await _settingsService.GetAmountToWordsOptionsAsync();
                AmountPrefix = options.Prefix;
                AmountSuffix = options.Suffix;
                CentsWord = options.CentsWord;
                UseAnd = options.UseAnd;
                OnPropertyChanged(nameof(AmountPreview));
            }
            catch
            {
                // Fallback to defaults
            }

            var info = await _licenceService.GetLicenceInfoAsync();
            LicenceKey = info.LicenceKey;
            LicenceStatus = info.Status;
            RemainingGraceDays = info.RemainingGraceDays;
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            try
            {
                var options = new AmountToWordsOptions
                {
                    Prefix = AmountPrefix?.Trim() ?? string.Empty,
                    Suffix = AmountSuffix?.Trim() ?? string.Empty,
                    CentsWord = CentsWord?.Trim() ?? string.Empty,
                    UseAnd = UseAnd
                };

                await _settingsService.SaveAmountToWordsOptionsAsync(options);
                IsSaveSuccess = true;
                SaveStatusMessage = "Settings saved successfully!";
                OnPropertyChanged(nameof(AmountPreview));
            }
            catch (Exception ex)
            {
                IsSaveSuccess = false;
                SaveStatusMessage = $"Failed to save settings: {ex.Message}";
            }
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
