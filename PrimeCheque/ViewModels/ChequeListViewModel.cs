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
    public partial class ChequeListViewModel : ObservableObject
    {
        private readonly IChequeService _chequeService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Cheque> _cheques = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private ChequeStatus? _selectedStatusFilter;

        public Array StatusValues => Enum.GetValues(typeof(ChequeStatus));

        public ChequeListViewModel(IChequeService chequeService, INavigationService navigationService)
        {
            _chequeService = chequeService;
            _navigationService = navigationService;
        }

        public async Task LoadChequesAsync()
        {
            var list = await _chequeService.GetChequesAsync(status: SelectedStatusFilter, searchQuery: SearchQuery);
            Cheques.Clear();
            foreach (var c in list) Cheques.Add(c);
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            await LoadChequesAsync();
        }

        [RelayCommand]
        private void PrintCheque(Cheque? cheque)
        {
            if (cheque == null) return;
            _navigationService.Navigate(typeof(PrintPreviewPage), cheque.Id);
        }

        [RelayCommand]
        private async Task VoidChequeAsync(Cheque? cheque)
        {
            if (cheque == null) return;
            await _chequeService.VoidChequeAsync(cheque.Id, "User", "User manual void");
            await LoadChequesAsync();
        }

        [RelayCommand]
        private async Task SubmitForApprovalAsync(Cheque? cheque)
        {
            if (cheque == null) return;
            await _chequeService.SubmitForApprovalAsync(cheque.Id, "Maker");
            await LoadChequesAsync();
        }

        [RelayCommand]
        private async Task ApproveChequeAsync(Cheque? cheque)
        {
            if (cheque == null) return;
            await _chequeService.ApproveChequeAsync(cheque.Id, "Checker");
            await LoadChequesAsync();
        }

        [RelayCommand]
        private async Task RejectChequeAsync(Cheque? cheque)
        {
            if (cheque == null) return;
            await _chequeService.RejectChequeAsync(cheque.Id, "Checker", "Amount mismatch or incomplete details");
            await LoadChequesAsync();
        }
    }
}
