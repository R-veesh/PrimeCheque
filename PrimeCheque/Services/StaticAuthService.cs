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
    public class StaticAuthService : IStaticAuthService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public StaticAuthService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            if (user == null) return null;

            // Check lockout
            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
            {
                return null;
            }

            var hash = HashPassword(password);
            if (user.PasswordHash == hash)
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.FailedLoginAttempts = 0;
                user.LockedUntil = null;
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

        public async Task<string?> GetSecurityQuestionAsync(string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            return user?.SecurityQuestion;
        }

        public async Task<bool> ValidateSecurityAnswerAsync(string username, string answer)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            if (user == null || string.IsNullOrEmpty(user.SecurityAnswerHash)) return false;

            var hash = HashPassword(answer.Trim().ToLowerInvariant());
            return user.SecurityAnswerHash == hash;
        }

        public async Task<bool> ResetPasswordAsync(string username, string newPassword)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            if (user == null) return false;

            user.PasswordHash = HashPassword(newPassword);
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            user.MustChangePassword = false;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return false;

            var currentHash = HashPassword(currentPassword);
            if (user.PasswordHash != currentHash) return false;

            user.PasswordHash = HashPassword(newPassword);
            user.MustChangePassword = false;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetSecurityQuestionAsync(Guid userId, string question, string answer)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return false;

            user.SecurityQuestion = question;
            user.SecurityAnswerHash = HashPassword(answer.Trim().ToLowerInvariant());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task ClearMustChangePasswordAsync(Guid userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return;

            user.MustChangePassword = false;
            await _dbContext.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}
