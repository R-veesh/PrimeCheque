using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class ChequeEntryPage : Page
    {
        public ChequeEntryViewModel ViewModel { get; }

        public ChequeEntryPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<ChequeEntryViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Guid chequeId)
            {
                await ViewModel.LoadExistingChequeAsync(chequeId);
            }
            else
            {
                await ViewModel.LoadDataAsync();
            }
        }
    }
}
