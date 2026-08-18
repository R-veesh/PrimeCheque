using System.Threading.Tasks;

namespace PrimeCheque.Services.Interfaces
{
    public interface IActivationService
    {
        string GetMachineId();
        Task<bool> IsCompanyActivatedAsync();
        Task<string?> RequestActivationAsync();
        Task<bool> PollActivationStatusAsync(string requestId);
    }
}
