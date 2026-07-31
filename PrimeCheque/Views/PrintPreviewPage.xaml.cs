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
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.PdfFilePath) && !string.IsNullOrEmpty(ViewModel.PdfFilePath))
            {
                try
                {
                    await PdfWebView.EnsureCoreWebView2Async();
                    if (System.IO.File.Exists(ViewModel.PdfFilePath))
                    {
                        PdfWebView.CoreWebView2.Navigate(new Uri(ViewModel.PdfFilePath).AbsoluteUri);
                    }
                }
                catch
                {
                    // Ignore navigation errors
                }
            }
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
