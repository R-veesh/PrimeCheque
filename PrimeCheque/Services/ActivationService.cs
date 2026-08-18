using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class ActivationService : IActivationService
    {
        private readonly HttpClient _httpClient;
        private string? _machineId;
        private const string MachineIdFile = @"C:\ProgramData\PrimeOne\PrimeCheque\machine_id.txt";

        // In a real scenario, this URL would come from AppSettings
        private const string BaseUrl = "http://127.0.0.1:8000/api/v1";

        public ActivationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl + "/");
        }

        public string GetMachineId()
        {
            if (!string.IsNullOrEmpty(_machineId)) return _machineId;

            try
            {
                if (System.IO.File.Exists(MachineIdFile))
                {
                    _machineId = System.IO.File.ReadAllText(MachineIdFile).Trim();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(MachineIdFile)!);
                    _machineId = Guid.NewGuid().ToString();
                    System.IO.File.WriteAllText(MachineIdFile, _machineId);
                }
            }
            catch
            {
                _machineId = Guid.NewGuid().ToString(); // Fallback for permission errors
            }

            return _machineId;
        }

        public async Task<bool> IsCompanyActivatedAsync()
        {
            // For checking activation, we could add a new endpoint /api/v1/installations/{machineId}/status
            // Let's implement that or assume false for now if not implemented.
            // Wait, we need to know if this specific installation is activated.
            // Let's check with the backend.
            try
            {
                var response = await _httpClient.GetAsync($"installations/{GetMachineId()}/status");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<InstallationStatusResponse>();
                    return result?.Status == "ACTIVE";
                }
                return false;
            }
            catch
            {
                // If offline, check a local cached state
                // For simplicity, we return false if we can't reach the server initially, requiring online activation
                // Or true if we have a locally cached ACTIVE state.
                var cachedStateFile = @"C:\ProgramData\PrimeOne\PrimeCheque\activation_state.txt";
                if (System.IO.File.Exists(cachedStateFile))
                {
                    var state = System.IO.File.ReadAllText(cachedStateFile);
                    return state.Trim() == "ACTIVE";
                }
                return false;
            }
        }

        public async Task<string?> RequestActivationAsync()
        {
            try
            {
                var payload = new 
                { 
                    installation_id = GetMachineId(),
                    platform = "Windows Desktop",
                    application_version = "1.0.0"
                };
                
                var response = await _httpClient.PostAsJsonAsync("activation/request", payload);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ActivationRequestResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.ActivationUrl))
                    {
                        Process.Start(new ProcessStartInfo(result.ActivationUrl) { UseShellExecute = true });
                        return result.RequestId;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> PollActivationStatusAsync(string requestId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"activation/status/{requestId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ActivationStatusResponse>();
                    if (result?.Status == "ACTIVE")
                    {
                        // Cache the active state locally for offline resilience
                        var cachedStateFile = @"C:\ProgramData\PrimeOne\PrimeCheque\activation_state.txt";
                        System.IO.File.WriteAllText(cachedStateFile, "ACTIVE");
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private class ActivationRequestResponse
        {
            [JsonPropertyName("request_id")]
            public string? RequestId { get; set; }
            [JsonPropertyName("activation_token")]
            public string? ActivationToken { get; set; }
            [JsonPropertyName("activation_url")]
            public string? ActivationUrl { get; set; }
        }

        private class ActivationStatusResponse
        {
            [JsonPropertyName("status")]
            public string? Status { get; set; }
        }

        private class InstallationStatusResponse
        {
            [JsonPropertyName("status")]
            public string? Status { get; set; }
        }
    }
}
