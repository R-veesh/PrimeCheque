using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data;

namespace PrimeCheque.Database
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(PrimeChequeDbContext dbContext)
        {
            // Ensure database file & EF schema structures exist
            await dbContext.Database.EnsureCreatedAsync();

            // Tables migration check for pre-existing SQLite databases
            var tableMigrations = new[]
            {
                @"CREATE TABLE IF NOT EXISTS ""PrinterCalibrations"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""PrinterName"" TEXT NOT NULL,
                    ""TrayName"" TEXT NULL,
                    ""HorizontalOffsetMm"" TEXT NOT NULL,
                    ""VerticalOffsetMm"" TEXT NOT NULL,
                    ""TemplateId"" TEXT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NULL
                );",
                @"CREATE TABLE IF NOT EXISTS ""ChequeAuditLogs"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""ChequeId"" TEXT NOT NULL,
                    ""ActionType"" TEXT NOT NULL,
                    ""PerformedBy"" TEXT NOT NULL,
                    ""Timestamp"" TEXT NOT NULL,
                    ""BeforeState"" TEXT NULL,
                    ""AfterState"" TEXT NULL,
                    FOREIGN KEY(""ChequeId"") REFERENCES ""Cheques""(""Id"") ON DELETE CASCADE
                );",
                @"CREATE TABLE IF NOT EXISTS ""Users"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""Username"" TEXT NOT NULL,
                    ""PasswordHash"" TEXT NOT NULL,
                    ""DisplayName"" TEXT NOT NULL,
                    ""Role"" TEXT NOT NULL,
                    ""IsActive"" INTEGER NOT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""LastLoginAt"" TEXT NULL,
                    ""FailedLoginAttempts"" INTEGER NOT NULL,
                    ""LockedUntil"" TEXT NULL
                );"
            };

            foreach (var sql in tableMigrations)
            {
                try
                {
                    await dbContext.Database.ExecuteSqlRawAsync(sql);
                }
                catch
                {
                    // Table already exists or structure up-to-date
                }
            }

            // Column migrations for Cheques table
            var columnMigrations = new[]
            {
                "ALTER TABLE Cheques ADD COLUMN RejectionReason TEXT;",
                "ALTER TABLE Cheques ADD COLUMN CreatedBy TEXT;",
                "ALTER TABLE Cheques ADD COLUMN ApprovedBy TEXT;",
                "ALTER TABLE Cheques ADD COLUMN PrintedAt TEXT;",
                "ALTER TABLE Cheques ADD COLUMN PdfPath TEXT;",
                "ALTER TABLE Cheques ADD COLUMN UpdatedAt TEXT;"
            };

            foreach (var sql in columnMigrations)
            {
                try
                {
                    await dbContext.Database.ExecuteSqlRawAsync(sql);
                }
                catch
                {
                    // Column already exists
                }
            }
        }
    }
}
