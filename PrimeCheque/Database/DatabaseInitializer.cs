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
        }
    }
}
