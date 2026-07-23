using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class PayeeManagementPage : Page
    {
        public PayeeManagementViewModel ViewModel { get; }

        public PayeeManagementPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<PayeeManagementViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataAsync();
        }
    }
}
