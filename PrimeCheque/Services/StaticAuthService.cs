using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class StaticAuthService : IStaticAuthService
    {
        private readonly List<StaticUser> _users = new()
        {
            new("admin", HashPassword("admin123"), "Administrator", UserRole.Administrator),
            new("preparer", HashPassword("preparer123"), "Cheque Preparer", UserRole.ChequePreparer),
            new("approver", HashPassword("approver123"), "Approver", UserRole.Approver),
            new("printer", HashPassword("printer123"), "Printer", UserRole.Printer),
            new("auditor", HashPassword("auditor123"), "Auditor", UserRole.Auditor),
        };

        public User? Authenticate(string username, string password)
        {
            var hash = HashPassword(password);
            var match = _users.FirstOrDefault(u =>
                u.Username == username && u.PasswordHash == hash);

            if (match == null) return null;

            return new User
            {
                Username = match.Username,
                PasswordHash = match.PasswordHash,
                DisplayName = match.DisplayName,
                Role = match.Role,
                IsActive = true
            };
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        private record StaticUser(string Username, string PasswordHash, string DisplayName, UserRole Role);
    }
}
