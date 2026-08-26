using System;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Gets the Super Admin user profile.
        /// </summary>
        Task<User?> GetAdminAsync();

        /// <summary>
        /// Updates the Super Admin display name.
        /// </summary>
        Task<User> UpdateAdminProfileAsync(string displayName);

        /// <summary>
        /// Authenticates the Super Admin (used for session login).
        /// </summary>
        Task<User?> AuthenticateAsync(string username, string password);
    }
}
