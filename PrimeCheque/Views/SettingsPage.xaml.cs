using Microsoft.UI.Xaml.Controls;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<SettingsViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadSettingsAsync();
        }
    }
}
