using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Helpers;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace PrimeCheque.ViewModels
{
    public partial class BatchImportViewModel : ObservableObject
    {
        private readonly IChequeService _chequeService;
        private readonly IChequeBookService _chequeBookService;
        private readonly ICompanyService _companyService;
        private readonly IAmountToWordsService _amountToWordsService;

        [ObservableProperty]
        private ObservableCollection<Company> _companies = new();

        [ObservableProperty]
        private ObservableCollection<ChequeBook> _chequeBooks = new();

        [ObservableProperty]
        private ObservableCollection<BatchImportRow> _importedRows = new();

        [ObservableProperty]
        private Company? _selectedCompany;

        [ObservableProperty]
        private ChequeBook? _selectedChequeBook;

        [ObservableProperty]
        private string _filePath = string.Empty;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private int _validCount;

        [ObservableProperty]
        private decimal _totalAmount;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        public BatchImportViewModel(
            IChequeService chequeService,
            IChequeBookService chequeBookService,
            ICompanyService companyService,
            IAmountToWordsService amountToWordsService)
        {
            _chequeService = chequeService;
            _chequeBookService = chequeBookService;
            _companyService = companyService;
            _amountToWordsService = amountToWordsService;
        }

        public async Task LoadDataAsync()
        {
            var comps = await _companyService.GetAllCompaniesAsync();
            Companies.Clear();
            foreach (var c in comps) Companies.Add(c);

            if (Companies.Count > 0)
            {
                SelectedCompany = Companies[0];
                var books = await _chequeBookService.GetChequeBooksByCompanyAsync(SelectedCompany.Id);
                ChequeBooks.Clear();
                foreach (var b in books) ChequeBooks.Add(b);
                if (ChequeBooks.Count > 0) SelectedChequeBook = ChequeBooks[0];
            }
        }

        [RelayCommand]
        private async Task BrowseAndParseFileAsync()
        {
            try
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".csv");
                picker.FileTypeFilter.Add(".txt");
                picker.FileTypeFilter.Add(".xml");
                picker.FileTypeFilter.Add(".xlsx");
                picker.FileTypeFilter.Add(".xls");

                // Retrieve window handle for WinUI 3 file picker
                var hwnd = WindowNative.GetWindowHandle(App.Current);
                InitializeWithWindow.Initialize(picker, hwnd);

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    FilePath = file.Path;
                    var rows = await ExcelImportHelper.ParseExcelOrCsvAsync(FilePath);
                    ImportedRows.Clear();
                    foreach (var r in rows) ImportedRows.Add(r);

                    TotalCount = ImportedRows.Count;
                    ValidCount = ImportedRows.Count(r => r.IsValid);
                    TotalAmount = ImportedRows.Where(r => r.IsValid).Sum(r => r.Amount);

                    StatusMessage = $"Loaded {TotalCount} rows ({ValidCount} valid).";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to read file: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ProcessBatchImportAsync()
        {
            if (SelectedCompany == null || SelectedChequeBook == null || ValidCount == 0)
            {
                StatusMessage = "Please select Company, Cheque Book, and load valid rows.";
                return;
            }

            int created = 0;
            foreach (var row in ImportedRows.Where(r => r.IsValid))
            {
                var words = _amountToWordsService.Convert(row.Amount);

                var cheque = new Cheque
                {
                    CompanyId = SelectedCompany.Id,
                    ChequeBookId = SelectedChequeBook.Id,
                    ChequeNumber = SelectedChequeBook.CurrentChequeNo,
                    PayeeName = row.PayeeName,
                    Amount = row.Amount,
                    AmountInWords = words,
                    ChequeDate = row.ChequeDate,
                    Memo = row.Memo,
                    CrossingType = row.CrossingType,
                    Status = ChequeStatus.Draft
                };

                await _chequeService.CreateChequeAsync(cheque, "Batch Import");
                created++;
            }

            StatusMessage = $"Batch complete! {created} draft cheques created.";
            ImportedRows.Clear();
            TotalCount = 0;
            ValidCount = 0;
            TotalAmount = 0;
        }
    }
}
