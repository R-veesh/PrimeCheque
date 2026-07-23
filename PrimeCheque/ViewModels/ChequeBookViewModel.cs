using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Helpers;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class ChequeBookViewModel : ObservableObject
    {
        private readonly IChequeBookService _chequeBookService;
        private readonly ICompanyService _companyService;
        private readonly IBankService _bankService;

        [ObservableProperty]
        private ObservableCollection<ChequeBook> _chequeBooks = new();

        [ObservableProperty]
        private ObservableCollection<Company> _companies = new();

        [ObservableProperty]
        private ObservableCollection<Bank> _banks = new();

        [ObservableProperty]
        private Company? _selectedCompany;

        [ObservableProperty]
        private Bank? _selectedBank;

        [ObservableProperty]
        private string _accountNumber = string.Empty;

        [ObservableProperty]
        private int _startChequeNo;

        [ObservableProperty]
        private int _endChequeNo;

        public ChequeBookViewModel(IChequeBookService chequeBookService, ICompanyService companyService, IBankService bankService)
        {
            _chequeBookService = chequeBookService;
            _companyService = companyService;
            _bankService = bankService;
        }

        public async Task LoadDataAsync()
        {
            var comps = await _companyService.GetAllCompaniesAsync();
            Companies.Clear();
            foreach (var c in comps) Companies.Add(c);

            var bnks = await _bankService.GetAllBanksAsync();
            Banks.Clear();
            foreach (var b in bnks) Banks.Add(b);

            if (Companies.Count > 0)
            {
                SelectedCompany = Companies[0];
                await LoadChequeBooksAsync();
            }
        }

        public async Task LoadChequeBooksAsync()
        {
            if (SelectedCompany == null) return;
            var books = await _chequeBookService.GetChequeBooksByCompanyAsync(SelectedCompany.Id);
            ChequeBooks.Clear();
            foreach (var b in books) ChequeBooks.Add(b);
        }

        [RelayCommand]
        private async Task AddChequeBookAsync()
        {
            if (SelectedCompany == null || SelectedBank == null || string.IsNullOrWhiteSpace(AccountNumber) || StartChequeNo <= 0 || EndChequeNo < StartChequeNo)
                return;

            var encryptedAccount = EncryptionHelper.Encrypt(AccountNumber);
            var maskedAccount = EncryptionHelper.MaskAccountNumber(AccountNumber);

            var book = new ChequeBook
            {
                CompanyId = SelectedCompany.Id,
                BankId = SelectedBank.Id,
                AccountNumber = encryptedAccount,
                MaskedAccountNumber = maskedAccount,
                StartChequeNo = StartChequeNo,
                EndChequeNo = EndChequeNo,
                CurrentChequeNo = StartChequeNo,
                Status = ChequeBookStatus.Active
            };

            await _chequeBookService.AddChequeBookAsync(book);
            AccountNumber = string.Empty;
            StartChequeNo = 0;
            EndChequeNo = 0;

            await LoadChequeBooksAsync();
        }
    }
}
