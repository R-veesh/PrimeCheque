using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface ISessionService
    {
        User? CurrentUser { get; set; }
        bool IsLoggedIn { get; }
        void Logout();
    }
}
