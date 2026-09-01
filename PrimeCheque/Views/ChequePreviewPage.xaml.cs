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
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.PdfFilePath) && !string.IsNullOrEmpty(ViewModel.PdfFilePath))
            {
                await LoadPdfInWebViewAsync(ViewModel.PdfFilePath);
            }
        }

        private async System.Threading.Tasks.Task LoadPdfInWebViewAsync(string pdfPath)
        {
            try
            {
                if (string.IsNullOrEmpty(pdfPath) || !System.IO.File.Exists(pdfPath)) return;

                var userDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrimeCheque", "WebView2");
                System.IO.Directory.CreateDirectory(userDataFolder);

                if (PdfWebView.CoreWebView2 == null)
                {
                    var environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateWithOptionsAsync(null, userDataFolder, null);
                    await PdfWebView.EnsureCoreWebView2Async(environment);
                }

                if (PdfWebView.CoreWebView2 != null)
                {
                    var folder = System.IO.Path.GetDirectoryName(pdfPath);
                    var fileName = System.IO.Path.GetFileName(pdfPath);
                    if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(fileName))
                    {
                        PdfWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                            "primecheque.preview",
                            folder,
                            Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);

                        PdfWebView.CoreWebView2.Navigate($"https://primecheque.preview/{fileName}");
                    }
                    else
                    {
                        PdfWebView.CoreWebView2.Navigate(new Uri(pdfPath).AbsoluteUri);
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\ProgramData\PrimeOne\PrimeCheque\startup.log", $"[{DateTime.Now:O}] WebView2 Cheque Preview Error: {ex.Message}\n");
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
                    if (!string.IsNullOrEmpty(ViewModel.PdfFilePath))
                    {
                        await LoadPdfInWebViewAsync(ViewModel.PdfFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\ProgramData\PrimeOne\PrimeCheque\startup.log", $"[{DateTime.Now:O}] ChequePreview OnNavigatedTo Error: {ex.Message}\n");
            }
        }
    }
}
