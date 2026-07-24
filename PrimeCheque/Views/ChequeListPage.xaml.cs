using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class ChequeListPage : Page
    {
        public ChequeListViewModel ViewModel { get; }

        public ChequeListPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<ChequeListViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                await ViewModel.LoadChequesAsync();
            }
            catch
            {
                // Prevent async void unhandled exception from crashing the app
            }
        }
    }
}
