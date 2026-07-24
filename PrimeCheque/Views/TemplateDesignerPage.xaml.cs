using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

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
                // Prevent async void unhandled exception from crashing the app
            }
        }

        private void PreviewBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 100)
            {
                ViewModel.UpdateCanvasDimensions(e.NewSize.Width);
            }
        }
    }
}
