using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data.Configurations;
using PrimeCheque.Data.Seed;
using PrimeCheque.Models;
using System;
using System.IO;

namespace PrimeCheque.Data
{
    public class PrimeChequeDbContext : DbContext
    {
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Bank> Banks => Set<Bank>();
        public DbSet<ChequeBook> ChequeBooks => Set<ChequeBook>();
        public DbSet<Cheque> Cheques => Set<Cheque>();
        public DbSet<ChequeAuditLog> ChequeAuditLogs => Set<ChequeAuditLog>();
        public DbSet<BankTemplate> BankTemplates => Set<BankTemplate>();
        public DbSet<Payee> Payees => Set<Payee>();
        public DbSet<User> Users => Set<User>();
        public DbSet<PrinterCalibration> PrinterCalibrations => Set<PrinterCalibration>();
        public DbSet<AppSettings> AppSettings => Set<AppSettings>();

        public PrimeChequeDbContext()
        {
        }

        public PrimeChequeDbContext(DbContextOptions<PrimeChequeDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var explicitDbPath = @"D:\PrimeOneWork\C#\PrimeCheque\PrimeCheque\PrimeCheque.db";
                var localBinDb = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PrimeCheque.db");

                string dbPath;
                if (File.Exists(explicitDbPath))
                {
                    dbPath = explicitDbPath;
                }
                else if (File.Exists(localBinDb))
                {
                    dbPath = localBinDb;
                }
                else
                {
                    var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrimeCheque");
                    Directory.CreateDirectory(folder);
                    dbPath = Path.Combine(folder, "primecheque.db");
                }

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new ChequeBookConfiguration());
            modelBuilder.ApplyConfiguration(new ChequeConfiguration());
            modelBuilder.ApplyConfiguration(new ChequeAuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new BankTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new PayeeConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            // Seed Data
            modelBuilder.Entity<Bank>().HasData(BankSeedData.GetInitialBanks());
            modelBuilder.Entity<BankTemplate>().HasData(TemplateSeedData.GetInitialTemplates());
        }
    }
}
