using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class UserManagementPage : Page
    {
        public UserManagementViewModel ViewModel { get; }

        public UserManagementPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<UserManagementViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                await ViewModel.LoadProfileAsync();
            }
            catch
            {
                // Prevent async void unhandled exception from crashing the app
            }
        }
    }
}
