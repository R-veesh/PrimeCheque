using Microsoft.UI.Xaml.Controls;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class ActivationPage : Page
    {
        public ActivationViewModel ViewModel { get; }

        public ActivationPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<ActivationViewModel>();
        }
    }
}
