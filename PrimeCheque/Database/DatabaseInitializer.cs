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
            // Ensure database directory exists and database is created with schema
            await dbContext.Database.EnsureCreatedAsync();

            // Dynamic column migrations for existing databases created before schema additions
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
                    // Column already exists or table structure is up to date
                }
            }
        }
    }
}
