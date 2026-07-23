using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class PayeeManagementViewModel : ObservableObject
    {
        private readonly IPayeeService _payeeService;
        private readonly ICompanyService _companyService;

        [ObservableProperty]
        private ObservableCollection<Payee> _payees = new();

        [ObservableProperty]
        private ObservableCollection<Company> _companies = new();

        [ObservableProperty]
        private Company? _selectedCompany;

        [ObservableProperty]
        private string _payeeName = string.Empty;

        [ObservableProperty]
        private string _nickName = string.Empty;

        [ObservableProperty]
        private string _defaultMemo = string.Empty;

        [ObservableProperty]
        private bool _isFavourite;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        public PayeeManagementViewModel(IPayeeService payeeService, ICompanyService companyService)
        {
            _payeeService = payeeService;
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
                await LoadPayeesAsync();
            }
        }

        public async Task LoadPayeesAsync()
        {
            if (SelectedCompany == null) return;
            var list = await _payeeService.SearchPayeesAsync(SelectedCompany.Id, SearchQuery);
            Payees.Clear();
            foreach (var p in list) Payees.Add(p);
        }

        [RelayCommand]
        private async Task SavePayeeAsync()
        {
            if (SelectedCompany == null || string.IsNullOrWhiteSpace(PayeeName)) return;

            var payee = new Payee
            {
                CompanyId = SelectedCompany.Id,
                Name = PayeeName,
                NickName = NickName,
                DefaultMemo = DefaultMemo,
                IsFavourite = IsFavourite
            };

            await _payeeService.AddOrUpdatePayeeAsync(payee);

            PayeeName = string.Empty;
            NickName = string.Empty;
            DefaultMemo = string.Empty;
            IsFavourite = false;

            await LoadPayeesAsync();
        }

        [RelayCommand]
        private async Task ToggleFavouriteAsync(Payee? payee)
        {
            if (payee == null) return;
            payee.IsFavourite = !payee.IsFavourite;
            await _payeeService.AddOrUpdatePayeeAsync(payee);
            await LoadPayeesAsync();
        }
    }
}
