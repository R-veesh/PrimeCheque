using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace PrimeCheque.Views
{
    public sealed partial class TemplateDesignerPage : Page
    {
        public TemplateDesignerViewModel ViewModel { get; }

        public TemplateDesignerPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<TemplateDesignerViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                await ViewModel.LoadDataAsync();
            }
            catch
            {
            }
        }

        private void PreviewBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 100)
            {
                ViewModel.UpdateCanvasDimensions(e.NewSize.Width);
            }
        }

        private void ChequeCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.SelectFieldCommand.Execute(null);
        }

        private void Field_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ChequeFieldViewModel fieldVm)
            {
                ViewModel.SelectFieldCommand.Execute(fieldVm);
                e.Handled = true;
            }
        }

        private void Handle_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void Field_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ChequeFieldViewModel fieldVm)
            {
                double scale = ViewModel.ScaleFactor;
                if (scale > 0)
                {
                    fieldVm.ApplyDelta(e.Delta.Translation.X, e.Delta.Translation.Y);
                }
            }
        }

        private void ResizeHandle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ChequeFieldViewModel fieldVm)
            {
                double scale = ViewModel.ScaleFactor;
                if (scale > 0)
                {
                    fieldVm.ApplyResize(e.Delta.Translation.X, e.Delta.Translation.Y);
                }
            }
        }

        private void RotateHandle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ChequeFieldViewModel fieldVm)
            {
                fieldVm.ApplyAngleDelta(e.Delta.Translation.X, e.Delta.Translation.Y);
            }
        }

        private async void LoadOverlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = App.MainWindow;
                if (window == null) return;

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                var picker = new FileOpenPicker();
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".pdf");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    if (file.FileType.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        var imagePath = await ConvertPdfPageToImageAsync(file);
                        if (imagePath != null)
                        {
                            ViewModel.OverlayImagePath = imagePath;
                        }
                        else
                        {
                            ViewModel.StatusMessage = "Could not render PDF page as image.";
                        }
                    }
                    else
                    {
                        ViewModel.OverlayImagePath = file.Path;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"Error loading overlay: {ex.Message}";
            }
        }

        private static async Task<string?> ConvertPdfPageToImageAsync(StorageFile pdfFile)
        {
            try
            {
                var pdfDoc = await Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(pdfFile);
                if (pdfDoc.PageCount == 0) return null;

                var page = pdfDoc.GetPage(0);

                var tempDir = Path.Combine(Path.GetTempPath(), "PrimeCheque", "Overlays");
                Directory.CreateDirectory(tempDir);
                var outputPath = Path.Combine(tempDir, $"{pdfFile.DisplayName}_page1.png");

                if (File.Exists(outputPath)) return outputPath;

                using var stream = new InMemoryRandomAccessStream();
                await page.RenderToStreamAsync(stream);
                stream.Seek(0);

                using var fileStream = File.Create(outputPath);
                await stream.AsStreamForRead().CopyToAsync(fileStream);

                page.Dispose();
                return outputPath;
            }
            catch
            {
                return null;
            }
        }
    }
}
