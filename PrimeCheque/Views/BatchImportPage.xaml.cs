using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class BatchImportPage : Page
    {
        public BatchImportViewModel ViewModel { get; }

        public BatchImportPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<BatchImportViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataAsync();
        }
    }
}
