using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class BankManagementViewModel : ObservableObject
    {
        private readonly IBankService _bankService;

        [ObservableProperty]
        private ObservableCollection<Bank> _banks = new();

        [ObservableProperty]
        private Bank? _selectedBank;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _shortName = string.Empty;

        [ObservableProperty]
        private string _branchCode = string.Empty;

        [ObservableProperty]
        private string _swiftCode = string.Empty;

        public BankManagementViewModel(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task LoadBanksAsync()
        {
            var list = await _bankService.GetAllBanksAsync();
            Banks.Clear();
            foreach (var b in list)
            {
                Banks.Add(b);
            }
        }

        [RelayCommand]
        private async Task SaveBankAsync()
        {
            if (string.IsNullOrWhiteSpace(Name)) return;

            var bank = SelectedBank ?? new Bank();
            bank.Name = Name;
            bank.ShortName = ShortName;
            bank.BranchCode = BranchCode;
            bank.SwiftCode = SwiftCode;

            if (SelectedBank == null)
            {
                await _bankService.CreateBankAsync(bank);
            }
            else
            {
                await _bankService.UpdateBankAsync(bank);
            }

            ClearForm();
            await LoadBanksAsync();
        }

        [RelayCommand]
        private void EditBank(Bank? bank)
        {
            if (bank == null) return;
            SelectedBank = bank;
            Name = bank.Name;
            ShortName = bank.ShortName ?? string.Empty;
            BranchCode = bank.BranchCode ?? string.Empty;
            SwiftCode = bank.SwiftCode ?? string.Empty;
        }

        [RelayCommand]
        private void ClearForm()
        {
            SelectedBank = null;
            Name = string.Empty;
            ShortName = string.Empty;
            BranchCode = string.Empty;
            SwiftCode = string.Empty;
        }
    }
}
