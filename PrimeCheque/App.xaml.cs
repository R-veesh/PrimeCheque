using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PrimeCheque.Data;
using PrimeCheque.Database;
using PrimeCheque.Services;
using PrimeCheque.Services.Interfaces;
using PrimeCheque.ViewModels;
using PrimeCheque.Views;

namespace PrimeCheque
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public static Window MainWindow { get; private set; } = null!;

        public App()
        {
            InitializeComponent();
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // DbContext
            services.AddDbContext<PrimeChequeDbContext>();

            // Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IStaticAuthService, StaticAuthService>();
            services.AddSingleton<ISessionService, SessionService>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IBankService, BankService>();
            services.AddTransient<IChequeBookService, ChequeBookService>();
            services.AddTransient<IPayeeService, PayeeService>();
            services.AddTransient<IAmountToWordsService, AmountToWordsService>();
            services.AddTransient<IChequeService, ChequeService>();
            services.AddTransient<IAuditService, AuditService>();
            services.AddTransient<ITemplateService, TemplateService>();
            services.AddTransient<IPdfGenerationService, PdfGenerationService>();
            services.AddTransient<IPrintService, PrintService>();
            services.AddTransient<IBackupService, BackupService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddSingleton<ILicenceService, LicenceService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<CompanyManagementViewModel>();
            services.AddTransient<BankManagementViewModel>();
            services.AddTransient<ChequeBookViewModel>();
            services.AddTransient<PayeeManagementViewModel>();
            services.AddTransient<ChequeEntryViewModel>();
            services.AddTransient<PrintPreviewViewModel>();
            services.AddTransient<ChequePreviewViewModel>();
            services.AddTransient<ChequeListViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<TemplateDesignerViewModel>();
            services.AddTransient<BatchImportViewModel>();
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<AuditLogViewModel>();
            services.AddTransient<UserManagementViewModel>();

            return services.BuildServiceProvider();
        }

        public static T GetService<T>() where T : class
        {
            return Services.GetRequiredService<T>();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PrimeChequeDbContext>();
                await DatabaseInitializer.InitializeAsync(dbContext);
            }

            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}
