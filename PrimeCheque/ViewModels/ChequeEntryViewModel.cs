using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;
using PrimeCheque.Views;

namespace PrimeCheque.ViewModels
{
    public partial class ChequeEntryViewModel : ObservableObject
    {
        private readonly IChequeService _chequeService;
        private readonly IChequeBookService _chequeBookService;
        private readonly ICompanyService _companyService;
        private readonly IPayeeService _payeeService;
        private readonly IAmountToWordsService _amountToWordsService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Company> _companies = new();

        [ObservableProperty]
        private ObservableCollection<ChequeBook> _chequeBooks = new();

        [ObservableProperty]
        private ObservableCollection<Payee> _payees = new();

        [ObservableProperty]
        private Company? _selectedCompany;

        [ObservableProperty]
        private ChequeBook? _selectedChequeBook;

        [ObservableProperty]
        private int _chequeNumber;

        [ObservableProperty]
        private string _payeeName = string.Empty;

        [ObservableProperty]
        private double _amount;

        [ObservableProperty]
        private string _amountInWords = string.Empty;

        [ObservableProperty]
        private DateTimeOffset _chequeDate = DateTimeOffset.Now;

        [ObservableProperty]
        private string _memo = string.Empty;

        [ObservableProperty]
        private CrossingType _selectedCrossing = CrossingType.AccountPayeeOnly;

        public Array CrossingTypes => Enum.GetValues(typeof(CrossingType));

        public ChequeEntryViewModel(
            IChequeService chequeService,
            IChequeBookService chequeBookService,
            ICompanyService companyService,
            IPayeeService payeeService,
            IAmountToWordsService amountToWordsService,
            INavigationService navigationService)
        {
            _chequeService = chequeService;
            _chequeBookService = chequeBookService;
            _companyService = companyService;
            _payeeService = payeeService;
            _amountToWordsService = amountToWordsService;
            _navigationService = navigationService;
        }

        public async Task LoadDataAsync()
        {
            var comps = await _companyService.GetAllCompaniesAsync();
            Companies.Clear();
            foreach (var c in comps) Companies.Add(c);

            if (Companies.Count > 0)
            {
                SelectedCompany = Companies[0];
                await OnCompanyChangedAsync();
            }
        }

        async partial void OnSelectedCompanyChanged(Company? value)
        {
            await OnCompanyChangedAsync();
        }

        private async Task OnCompanyChangedAsync()
        {
            if (SelectedCompany == null) return;

            var books = await _chequeBookService.GetChequeBooksByCompanyAsync(SelectedCompany.Id);
            ChequeBooks.Clear();
            foreach (var b in books) ChequeBooks.Add(b);

            if (ChequeBooks.Count > 0)
            {
                SelectedChequeBook = ChequeBooks[0];
            }

            var payeeList = await _payeeService.GetPayeesByCompanyAsync(SelectedCompany.Id);
            Payees.Clear();
            foreach (var p in payeeList) Payees.Add(p);
        }

        partial void OnSelectedChequeBookChanged(ChequeBook? value)
        {
            if (value != null)
            {
                ChequeNumber = value.CurrentChequeNo;
            }
        }

        partial void OnAmountChanged(double value)
        {
            if (value >= 0)
            {
                AmountInWords = _amountToWordsService.Convert((decimal)value);
            }
        }

        [RelayCommand]
        private async Task SaveDraftAsync()
        {
            if (SelectedCompany == null || SelectedChequeBook == null || string.IsNullOrWhiteSpace(PayeeName) || Amount <= 0)
                return;

            var cheque = new Cheque
            {
                CompanyId = SelectedCompany.Id,
                ChequeBookId = SelectedChequeBook.Id,
                ChequeNumber = ChequeNumber,
                PayeeName = PayeeName,
                Amount = (decimal)Amount,
                AmountInWords = AmountInWords,
                ChequeDate = DateOnly.FromDateTime(ChequeDate.DateTime),
                Memo = Memo,
                CrossingType = SelectedCrossing,
                Status = ChequeStatus.Draft
            };

            await _chequeService.CreateChequeAsync(cheque, "User");
            _navigationService.Navigate(typeof(ChequeListPage));
        }

        [RelayCommand]
        private async Task PreviewAndPrintAsync()
        {
            if (SelectedCompany == null || SelectedChequeBook == null || string.IsNullOrWhiteSpace(PayeeName) || Amount <= 0)
                return;

            var cheque = new Cheque
            {
                CompanyId = SelectedCompany.Id,
                ChequeBookId = SelectedChequeBook.Id,
                ChequeNumber = ChequeNumber,
                PayeeName = PayeeName,
                Amount = (decimal)Amount,
                AmountInWords = AmountInWords,
                ChequeDate = DateOnly.FromDateTime(ChequeDate.DateTime),
                Memo = Memo,
                CrossingType = SelectedCrossing,
                Status = ChequeStatus.Draft
            };

            var created = await _chequeService.CreateChequeAsync(cheque, "User");
            _navigationService.Navigate(typeof(PrintPreviewPage), created.Id);
        }
    }
}
