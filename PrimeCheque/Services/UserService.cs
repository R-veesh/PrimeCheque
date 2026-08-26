using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class UserService : IUserService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public UserService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetAdminAsync()
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.IsActive);
        }

        public async Task<User> UpdateAdminProfileAsync(string displayName)
        {
            var admin = await _dbContext.Users.FirstOrDefaultAsync(u => u.IsActive);
            if (admin == null)
                throw new InvalidOperationException("No active Super Admin user found.");

            admin.DisplayName = displayName;
            await _dbContext.SaveChangesAsync();
            return admin;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            if (user == null) return null;

            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
            {
                return null; // Locked
            }

            var hash = HashPassword(password);
            if (user.PasswordHash == hash)
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.FailedLoginAttempts = 0;
                await _dbContext.SaveChangesAsync();
                return user;
            }
            else
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                }
                await _dbContext.SaveChangesAsync();
                return null;
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
