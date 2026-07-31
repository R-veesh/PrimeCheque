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

        private Guid? _editingChequeId;

        [ObservableProperty]
        private bool _isEditMode;

        public string PageTitle => IsEditMode ? "Edit Cheque Entry" : "New Cheque Entry";

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

        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<CrossingType, string>> CrossingTypes => new()
        {
            new(CrossingType.CrossAccountPayeeAndOrBearer, "Cross A/C Payee + Or Bearer"),
            new(CrossingType.CrossAccountPayeeAndNotNegotiableAndOrBearer, "Cross A/C Payee + Not Negotiable + Or Bearer"),
            new(CrossingType.AccountPayeeOnly, "Cross A/C Payee"),
            new(CrossingType.CrossOnly, "Cross Only"),
            new(CrossingType.None, "No Crossing")
        };

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

        public async Task LoadExistingChequeAsync(Guid chequeId)
        {
            var cheque = await _chequeService.GetChequeByIdAsync(chequeId);
            if (cheque == null) return;

            // Load dependencies first
            await LoadDataAsync();

            _editingChequeId = cheque.Id;
            IsEditMode = true;
            OnPropertyChanged(nameof(PageTitle));

            // Select matching company
            foreach (var comp in Companies)
            {
                if (comp.Id == cheque.CompanyId)
                {
                    SelectedCompany = comp;
                    break;
                }
            }

            // Await company change to load chequebooks and payees
            await OnCompanyChangedAsync();

            // Select matching chequebook
            foreach (var book in ChequeBooks)
            {
                if (book.Id == cheque.ChequeBookId)
                {
                    SelectedChequeBook = book;
                    break;
                }
            }

            ChequeNumber = cheque.ChequeNumber;
            PayeeName = cheque.PayeeName;
            Amount = (double)cheque.Amount;
            ChequeDate = cheque.ChequeDate.ToDateTime(new TimeOnly(0, 0));
            SelectedCrossing = cheque.CrossingType;
            Memo = cheque.Memo ?? string.Empty;
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

            var cheque = IsEditMode ? await _chequeService.GetChequeByIdAsync(_editingChequeId!.Value) : new Cheque();
            if (cheque == null) return;

            cheque.CompanyId = SelectedCompany.Id;
            cheque.ChequeBookId = SelectedChequeBook.Id;
            cheque.ChequeNumber = ChequeNumber;
            cheque.PayeeName = PayeeName;
            cheque.Amount = (decimal)Amount;
            cheque.AmountInWords = AmountInWords;
            cheque.ChequeDate = DateOnly.FromDateTime(ChequeDate.DateTime);
            cheque.Memo = Memo;
            cheque.CrossingType = SelectedCrossing;
            cheque.Status = ChequeStatus.Draft;

            if (IsEditMode)
                await _chequeService.UpdateChequeAsync(cheque, "User");
            else
                await _chequeService.CreateChequeAsync(cheque, "User");

            _navigationService.Navigate(typeof(ChequeListPage));
        }

        [RelayCommand]
        private async Task PreviewAndPrintAsync()
        {
            if (SelectedCompany == null || SelectedChequeBook == null || string.IsNullOrWhiteSpace(PayeeName) || Amount <= 0)
                return;

            var cheque = IsEditMode ? await _chequeService.GetChequeByIdAsync(_editingChequeId!.Value) : new Cheque();
            if (cheque == null) return;

            cheque.CompanyId = SelectedCompany.Id;
            cheque.ChequeBookId = SelectedChequeBook.Id;
            cheque.ChequeNumber = ChequeNumber;
            cheque.PayeeName = PayeeName;
            cheque.Amount = (decimal)Amount;
            cheque.AmountInWords = AmountInWords;
            cheque.ChequeDate = DateOnly.FromDateTime(ChequeDate.DateTime);
            cheque.Memo = Memo;
            cheque.CrossingType = SelectedCrossing;
            cheque.Status = ChequeStatus.Draft;

            if (IsEditMode)
                cheque = await _chequeService.UpdateChequeAsync(cheque, "User");
            else
                cheque = await _chequeService.CreateChequeAsync(cheque, "User");

            _navigationService.Navigate(typeof(PrintPreviewPage), cheque.Id);
        }

        [RelayCommand]
        private void GoBack()
        {
            _navigationService.GoBack();
        }
    }
}
