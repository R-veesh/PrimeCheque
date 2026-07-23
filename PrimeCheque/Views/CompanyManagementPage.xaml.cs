using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class CompanyManagementPage : Page
    {
        public CompanyManagementViewModel ViewModel { get; }

        public CompanyManagementPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<CompanyManagementViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadCompaniesAsync();
        }
    }
}
