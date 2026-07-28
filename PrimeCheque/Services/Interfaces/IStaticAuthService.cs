using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IStaticAuthService
    {
        User? Authenticate(string username, string password);
    }
}
