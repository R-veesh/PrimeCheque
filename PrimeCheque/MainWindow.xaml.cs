using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PrimeCheque.Services.Interfaces;
using PrimeCheque.Views;

namespace PrimeCheque
{
    public sealed partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;

        public MainWindow()
        {
            InitializeComponent();
            _navigationService = App.GetService<INavigationService>();
            _navigationService.Frame = ContentFrame;
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.SelectedItem = NavView.MenuItems[0];
            _navigationService.Navigate(typeof(DashboardPage));
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                _navigationService.Navigate(typeof(SettingsPage));
                return;
            }

            if (args.SelectedItem is NavigationViewItem item)
            {
                var tag = item.Tag?.ToString();
                switch (tag)
                {
                    case "Dashboard":
                        _navigationService.Navigate(typeof(DashboardPage));
                        break;
                    case "NewCheque":
                        _navigationService.Navigate(typeof(ChequeEntryPage));
                        break;
                    case "Cheques":
                        _navigationService.Navigate(typeof(ChequeListPage));
                        break;
                    case "ChequeBooks":
                        _navigationService.Navigate(typeof(ChequeBookPage));
                        break;
                    case "Payees":
                        _navigationService.Navigate(typeof(PayeeManagementPage));
                        break;
                    case "BatchImport":
                        _navigationService.Navigate(typeof(BatchImportPage));
                        break;
                    case "Reports":
                        _navigationService.Navigate(typeof(ReportsPage));
                        break;
                    case "AuditLog":
                        _navigationService.Navigate(typeof(AuditLogPage));
                        break;
                    case "Companies":
                        _navigationService.Navigate(typeof(CompanyManagementPage));
                        break;
                    case "Banks":
                        _navigationService.Navigate(typeof(BankManagementPage));
                        break;
                    case "TemplateDesigner":
                        _navigationService.Navigate(typeof(TemplateDesignerPage));
                        break;
                }
            }
        }
    }
}
