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
            await ViewModel.LoadDataAsync();
        }
    }
}
