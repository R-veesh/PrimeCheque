using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;
using PrimeCheque.Views;

namespace PrimeCheque
{
    public sealed partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;
        private readonly ISessionService _session;
        private readonly Dictionary<UserRole, List<string>> _rolePermissions;

        public MainWindow()
        {
            InitializeComponent();
            ConfigureCustomTitleBar();
            SetMinimumSize();

            // Set default size and center on screen
            AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 850));
            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(AppWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
            if (displayArea != null)
            {
                var workArea = displayArea.WorkArea;
                var position = new Windows.Graphics.PointInt32(
                    workArea.X + (workArea.Width - 1200) / 2,
                    workArea.Y + (workArea.Height - 850) / 2
                );
                AppWindow.Move(position);
            }

            _session = App.GetService<ISessionService>();
            _navigationService = App.GetService<INavigationService>();
            _navigationService.Frame = ContentFrame;

            _rolePermissions = new Dictionary<UserRole, List<string>>
            {
                [UserRole.Administrator] = new() { "Dashboard", "NewCheque", "Cheques", "ChequeBooks", "Payees", "BatchImport", "Reports", "AuditLog", "Companies", "Banks", "TemplateDesigner", "Users", "Settings" },
                [UserRole.ChequePreparer] = new() { "Dashboard", "NewCheque", "Cheques", "ChequeBooks", "Payees", "BatchImport", "Reports", "Settings" },
                [UserRole.Approver] = new() { "Dashboard", "Cheques", "Reports", "Settings" },
                [UserRole.Printer] = new() { "Dashboard", "Cheques", "Reports", "Settings" },
                [UserRole.Auditor] = new() { "Dashboard", "Cheques", "Reports", "AuditLog", "Settings" },
            };

            ShowLogin();
        }

        private void ShowLogin()
        {
            var loginPage = new LoginPage();
            loginPage.ViewModel.OnLoginSucceeded = ShowAppShell;
            LoginFrame.Content = loginPage;
            LoginFrame.Visibility = Visibility.Visible;
            NavView.Visibility = Visibility.Collapsed;
        }

        private void ShowAppShell()
        {
            LoginFrame.Visibility = Visibility.Collapsed;
            NavView.Visibility = Visibility.Visible;

            var user = _session.CurrentUser;
            if (user != null)
            {
                UserDisplayNameText.Text = user.DisplayName;
                UserRoleTextBlock.Text = user.Role.ToString();
                UserAvatarInitial.Text = user.DisplayName.Length > 0
                    ? user.DisplayName[..1].ToUpper()
                    : "?";
            }

            ApplyRoleVisibility();
            NavigateToDefaultPage();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyRoleVisibility();
            NavigateToDefaultPage();
        }

        private void NavigateToDefaultPage()
        {
            if (NavView.MenuItems.Count > 0)
            {
                NavView.SelectedItem = NavDashboard;
                _navigationService.Navigate(typeof(DashboardPage));
            }
        }

        private void ApplyRoleVisibility()
        {
            var role = _session.CurrentUser?.Role ?? UserRole.ChequePreparer;
            var allowed = _rolePermissions.GetValueOrDefault(role, new());

            NavView.IsSettingsVisible = allowed.Contains("Settings");

            foreach (var item in NavView.MenuItems)
            {
                if (item is NavigationViewItem navItem)
                {
                    var tag = navItem.Tag?.ToString();
                    navItem.Visibility = tag != null && allowed.Contains(tag)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
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
                    case "Users":
                        _navigationService.Navigate(typeof(UserManagementPage));
                        break;
                    case "Settings":
                        _navigationService.Navigate(typeof(SettingsPage));
                        break;
                }
            }
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            _navigationService.GoBack();
        }

        private void ConfigureCustomTitleBar()
        {
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            var titleBar = AppWindow.TitleBar;
            titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;

            AppWindow.Changed += AppWindow_Changed;
        }

        private void AppWindow_Changed(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowChangedEventArgs args)
        {
            if (args.DidSizeChange && sender.TitleBar.ExtendsContentIntoTitleBar)
            {
                // AppWindow properties are in physical pixels, but XAML requires logical pixels.
                // It is safer to maintain a fixed safe margin for the system caption buttons.
                TitleBarRightControls.Margin = new Thickness(0, 0, 160, 0);
            }
        }

        private void TitleBarSettings_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.Navigate(typeof(SettingsPage));
        }

        private void TitleBarThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            // Simple theme toggle logic placeholder
            if (Content is FrameworkElement fe)
            {
                fe.RequestedTheme = fe.RequestedTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _session.Logout();
            ShowLogin();
        }

        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        private WndProcDelegate _wndProcDelegate;
        private IntPtr _oldWndProc;

        [DllImport("user32.dll")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WndProcDelegate newProc);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, WndProcDelegate newProc);

        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);

        private const int GWLP_WNDPROC = -4;
        private const uint WM_GETMINMAXINFO = 0x0024;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);

                uint dpi = GetDpiForWindow(hWnd);
                float scalingFactor = (float)dpi / 96;

                int minWidth = 1400;
                int minHeight = 700;

                minMaxInfo.ptMinTrackSize.x = (int)(minWidth * scalingFactor);
                minMaxInfo.ptMinTrackSize.y = (int)(minHeight * scalingFactor);

                Marshal.StructureToPtr(minMaxInfo, lParam, true);
            }

            return CallWindowProc(_oldWndProc, hWnd, msg, wParam, lParam);
        }

        private void SetMinimumSize()
        {
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            _wndProcDelegate = new WndProcDelegate(WndProc);
            if (IntPtr.Size == 8)
                _oldWndProc = SetWindowLongPtr(hwnd, GWLP_WNDPROC, _wndProcDelegate);
            else
                _oldWndProc = SetWindowLong(hwnd, GWLP_WNDPROC, _wndProcDelegate);
        }
    }
}
