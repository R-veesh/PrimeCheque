using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data;
using PrimeCheque.Models;

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
                "ALTER TABLE Cheques ADD COLUMN UpdatedAt TEXT;",
                // Super Admin password reset columns
                "ALTER TABLE Users ADD COLUMN SecurityQuestion TEXT;",
                "ALTER TABLE Users ADD COLUMN SecurityAnswerHash TEXT;",
                "ALTER TABLE Users ADD COLUMN MustChangePassword INTEGER NOT NULL DEFAULT 1;"
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

            // Seed default Super Admin user if no users exist
            await SeedSuperAdminAsync(dbContext);
        }

        private static async Task SeedSuperAdminAsync(PrimeChequeDbContext dbContext)
        {
            var hasUsers = await dbContext.Users.AnyAsync();
            if (!hasUsers)
            {
                var superAdmin = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    DisplayName = "Super Administrator",
                    Role = UserRole.SuperAdmin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    MustChangePassword = true
                };

                dbContext.Users.Add(superAdmin);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                // Update existing users to SuperAdmin role if migrating from old schema
                var users = await dbContext.Users.ToListAsync();
                foreach (var user in users)
                {
                    user.Role = UserRole.SuperAdmin;
                }
                await dbContext.SaveChangesAsync();
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}
