using System;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IStaticAuthService
    {
        /// <summary>
        /// Authenticates the Super Admin against the database.
        /// </summary>
        Task<User?> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Gets the security question for password reset.
        /// </summary>
        Task<string?> GetSecurityQuestionAsync(string username);

        /// <summary>
        /// Validates the security answer for password reset.
        /// </summary>
        Task<bool> ValidateSecurityAnswerAsync(string username, string answer);

        /// <summary>
        /// Resets the password after security question verification.
        /// </summary>
        Task<bool> ResetPasswordAsync(string username, string newPassword);

        /// <summary>
        /// Changes the password with old password verification.
        /// </summary>
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);

        /// <summary>
        /// Sets or updates the security question and answer.
        /// </summary>
        Task<bool> SetSecurityQuestionAsync(Guid userId, string question, string answer);

        /// <summary>
        /// Marks the user as no longer needing to change password.
        /// </summary>
        Task ClearMustChangePasswordAsync(Guid userId);
    }
}
