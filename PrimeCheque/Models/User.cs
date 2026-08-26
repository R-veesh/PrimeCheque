using System;

namespace PrimeCheque.Models
{
    public enum UserRole
    {
        SuperAdmin
    }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.SuperAdmin;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }

        // Password Reset & Security
        public string? SecurityQuestion { get; set; }
        public string? SecurityAnswerHash { get; set; }
        public bool MustChangePassword { get; set; } = true;
    }
}
