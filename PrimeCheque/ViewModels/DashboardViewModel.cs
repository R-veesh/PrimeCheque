using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;
using PrimeCheque.Views;

namespace PrimeCheque.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IChequeService _chequeService;
        private readonly IChequeBookService _chequeBookService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private int _totalChequesToday;

        [ObservableProperty]
        private int _pendingApprovals;

        [ObservableProperty]
        private decimal _totalAmountToday;

        [ObservableProperty]
        private ObservableCollection<Cheque> _recentCheques = new();

        public DashboardViewModel(IChequeService chequeService, IChequeBookService chequeBookService, INavigationService navigationService)
        {
            _chequeService = chequeService;
            _chequeBookService = chequeBookService;
            _navigationService = navigationService;
        }

        public async Task LoadDataAsync()
        {
            var cheques = await _chequeService.GetChequesAsync();
            var today = DateOnly.FromDateTime(DateTime.Today);

            var todayCheques = cheques.Where(c => c.ChequeDate == today).ToList();
            TotalChequesToday = todayCheques.Count;
            TotalAmountToday = todayCheques.Sum(c => c.Amount);
            PendingApprovals = cheques.Count(c => c.Status == ChequeStatus.Draft);

            RecentCheques.Clear();
            foreach (var c in cheques.Take(10))
            {
                RecentCheques.Add(c);
            }
        }

        [RelayCommand]
        private void NavigateToNewCheque()
        {
            _navigationService.Navigate(typeof(ChequeEntryPage));
        }

        [RelayCommand]
        private void NavigateToChequeList()
        {
            _navigationService.Navigate(typeof(ChequeListPage));
        }

        [RelayCommand]
        private void PrintCheque(Cheque? cheque)
        {
            if (cheque == null) return;
            _navigationService.Navigate(typeof(PrintPreviewPage), cheque.Id);
        }
    }
}
