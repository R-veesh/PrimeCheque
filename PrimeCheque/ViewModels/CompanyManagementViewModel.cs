using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class CompanyManagementViewModel : ObservableObject
    {
        private readonly ICompanyService _companyService;

        [ObservableProperty]
        private ObservableCollection<Company> _companies = new();

        [ObservableProperty]
        private Company? _selectedCompany;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _registrationNumber = string.Empty;

        [ObservableProperty]
        private string _address = string.Empty;

        [ObservableProperty]
        private string _phone = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        public CompanyManagementViewModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task LoadCompaniesAsync()
        {
            var list = await _companyService.GetAllCompaniesAsync();
            Companies.Clear();
            foreach (var c in list)
            {
                Companies.Add(c);
            }
        }

        [RelayCommand]
        private async Task SaveCompanyAsync()
        {
            if (string.IsNullOrWhiteSpace(Name)) return;

            var company = SelectedCompany ?? new Company();
            company.Name = Name;
            company.RegistrationNumber = RegistrationNumber;
            company.Address = Address;
            company.Phone = Phone;
            company.Email = Email;

            if (SelectedCompany == null)
            {
                await _companyService.CreateCompanyAsync(company);
            }
            else
            {
                await _companyService.UpdateCompanyAsync(company);
            }

            ClearForm();
            await LoadCompaniesAsync();
        }

        [RelayCommand]
        private void EditCompany(Company? company)
        {
            if (company == null) return;
            SelectedCompany = company;
            Name = company.Name;
            RegistrationNumber = company.RegistrationNumber ?? string.Empty;
            Address = company.Address ?? string.Empty;
            Phone = company.Phone ?? string.Empty;
            Email = company.Email ?? string.Empty;
        }

        [RelayCommand]
        private void ClearForm()
        {
            SelectedCompany = null;
            Name = string.Empty;
            RegistrationNumber = string.Empty;
            Address = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
        }
    }
}
