using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PrimeCheque.ViewModels;

namespace PrimeCheque.Views
{
    public sealed partial class AuditLogPage : Page
    {
        public AuditLogViewModel ViewModel { get; }

        public AuditLogPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<AuditLogViewModel>();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadAuditLogsAsync();
        }
    }
}
