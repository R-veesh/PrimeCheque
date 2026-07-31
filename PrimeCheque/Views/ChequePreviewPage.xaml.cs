using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class ChequePreviewPage : Page
    {
        public ChequePreviewViewModel ViewModel { get; }

        public ChequePreviewPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<ChequePreviewViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                if (e.Parameter is Guid chequeId)
                {
                    await ViewModel.LoadChequeAsync(chequeId);
                }
            }
            catch
            {
                // Prevent async void exception from crashing the app
            }
        }
    }
}
