using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public enum ReportType
    {
        ChequeRegister,
        PostDatedCheques,
        VoidCheques
    }

    public partial class ReportsViewModel : ObservableObject
    {
        private readonly IReportService _reportService;
        private readonly ICompanyService _companyService;

        [ObservableProperty]
        private ObservableCollection<Company> _companies = new();

        [ObservableProperty]
        private ObservableCollection<Cheque> _reportData = new();

        [ObservableProperty]
        private Company? _selectedCompany;

        [ObservableProperty]
        private ReportType _selectedReportType = ReportType.ChequeRegister;

        [ObservableProperty]
        private DateTimeOffset _startDate = DateTimeOffset.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTimeOffset _endDate = DateTimeOffset.Now;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private decimal _totalAmount;

        public Array ReportTypes => Enum.GetValues(typeof(ReportType));

        public ReportsViewModel(IReportService reportService, ICompanyService companyService)
        {
            _reportService = reportService;
            _companyService = companyService;
        }

        public async Task LoadDataAsync()
        {
            var comps = await _companyService.GetAllCompaniesAsync();
            Companies.Clear();
            foreach (var c in comps) Companies.Add(c);

            if (Companies.Count > 0)
            {
                SelectedCompany = Companies[0];
                await GenerateReportAsync();
            }
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            if (SelectedCompany == null) return;

            ReportData.Clear();
            System.Collections.Generic.List<Cheque> list;

            switch (SelectedReportType)
            {
                case ReportType.PostDatedCheques:
                    list = await _reportService.GetPostDatedChequesAsync(SelectedCompany.Id, DateTime.Today);
                    break;
                case ReportType.VoidCheques:
                    list = await _reportService.GetVoidChequesAsync(SelectedCompany.Id, StartDate.DateTime, EndDate.DateTime);
                    break;
                case ReportType.ChequeRegister:
                default:
                    list = await _reportService.GetChequeRegisterAsync(SelectedCompany.Id, StartDate.DateTime, EndDate.DateTime);
                    break;
            }

            foreach (var c in list)
            {
                ReportData.Add(c);
            }

            TotalCount = ReportData.Count;
            TotalAmount = 0m;
            foreach (var item in ReportData)
            {
                TotalAmount += item.Amount;
            }
        }
    }
}
