using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class BankManagementPage : Page
    {
        public BankManagementViewModel ViewModel { get; }

        public BankManagementPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<BankManagementViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                await ViewModel.LoadBanksAsync();
            }
            catch
            {
                // Prevent async void unhandled exception from crashing the app
            }
        }
    }
}
