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
        private readonly ISessionService _sessionService;

        public bool IsPreparerRole => _sessionService.CurrentUser?.Role == UserRole.Administrator || _sessionService.CurrentUser?.Role == UserRole.ChequePreparer;
        public bool IsApproverRole => _sessionService.CurrentUser?.Role == UserRole.Administrator || _sessionService.CurrentUser?.Role == UserRole.Approver;
        public bool IsPrinterRole => _sessionService.CurrentUser?.Role == UserRole.Administrator || _sessionService.CurrentUser?.Role == UserRole.Printer;

        [ObservableProperty]
        private ObservableCollection<Cheque> _cheques = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private ChequeStatus? _selectedStatusFilter;

        public Array StatusValues => Enum.GetValues(typeof(ChequeStatus));

        public ChequeListViewModel(IChequeService chequeService, INavigationService navigationService, ISessionService sessionService)
        {
            _chequeService = chequeService;
            _navigationService = navigationService;
            _sessionService = sessionService;
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
        private void PreviewCheque(Cheque? cheque)
        {
            if (cheque == null) return;
            _navigationService.Navigate(typeof(ChequePreviewPage), cheque.Id);
        }

        [RelayCommand]
        private void PrintCheque(Cheque? cheque)
        {
            if (cheque == null) return;
            _navigationService.Navigate(typeof(PrintPreviewPage), cheque.Id);
        }

        [RelayCommand]
        private void EditCheque(Cheque? cheque)
        {
            if (cheque == null) return;
            _navigationService.Navigate(typeof(ChequeEntryPage), cheque.Id);
        }

        [RelayCommand]
        private async Task DeleteChequeAsync(Cheque? cheque)
        {
            if (cheque == null) return;
            var userName = _sessionService.CurrentUser?.Username ?? "System";
            await _chequeService.VoidChequeAsync(cheque.Id, userName, "Deleted by user");
            await LoadChequesAsync();
        }

        [RelayCommand]
        private async Task ApproveChequeAsync(Cheque? cheque)
        {
            if (cheque == null) return;
            var userName = _sessionService.CurrentUser?.Username ?? "Checker";
            await _chequeService.ApproveChequeAsync(cheque.Id, userName);
            _navigationService.Navigate(typeof(PrintPreviewPage), cheque.Id);
        }

        [RelayCommand]
        private async Task RejectChequeAsync(Cheque? cheque)
        {
            if (cheque == null) return;
            var userName = _sessionService.CurrentUser?.Username ?? "Checker";
            await _chequeService.RejectChequeAsync(cheque.Id, userName, "Amount mismatch or incomplete details");
            await LoadChequesAsync();
        }
    }
}
