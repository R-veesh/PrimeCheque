using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; }

        public LoginPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<LoginViewModel>();
            DataContext = ViewModel;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ViewModel.LoginCommand.Execute(null);
            }
        }
    }
}
