using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class PrintPreviewPage : Page
    {
        public PrintPreviewViewModel ViewModel { get; }

        public PrintPreviewPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<PrintPreviewViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Guid chequeId)
            {
                await ViewModel.LoadChequeAsync(chequeId);
            }
        }
    }
}
