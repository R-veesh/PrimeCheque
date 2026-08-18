using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class ApiIntegrationService : IApiIntegrationService
    {
        private readonly HttpClient _httpClient;
        private string? _bearerToken;

        // In a real scenario, this URL would come from AppSettings/Configuration
        private const string BaseUrl = "http://127.0.0.1:8000/api/v1";

        public ApiIntegrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl + "/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void EnsureAuthenticated()
        {
            if (!string.IsNullOrEmpty(_bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            }
        }

        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AuthenticateAsync(string licenceKey, string machineId)
        {
            try
            {
                var payload = new { licence_key = licenceKey, machine_id = machineId };
                var response = await _httpClient.PostAsJsonAsync("auth/token", payload);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<TokenResponse>>();
                    if (result?.Success == true && result.Data != null)
                    {
                        // Store the token (we only keep it in memory for now)
                        _bearerToken = result.Data.Token.Contains("|") ? result.Data.Token.Split('|')[1] : result.Data.Token;
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

        public async Task<LicenceStatusDto?> ActivateLicenceAsync(string licenceKey, string machineId, string machineName)
        {
            try
            {
                EnsureAuthenticated();
                var payload = new { licence_key = licenceKey, machine_id = machineId, machine_name = machineName };
                var response = await _httpClient.PostAsJsonAsync("licences/activate", payload);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<LicenceStatusDto>>();
                    if (result?.Success == true)
                    {
                        return result.Data;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<LicenceStatusDto?> GetLicenceStatusAsync()
        {
            try
            {
                EnsureAuthenticated();
                var response = await _httpClient.GetAsync("licences/status");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<LicenceStatusDto>>();
                    if (result?.Success == true)
                    {
                        return result.Data;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<BankTemplateDto>?> FetchTemplatesAsync()
        {
            try
            {
                EnsureAuthenticated();
                var response = await _httpClient.GetAsync("templates");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<BankTemplateDto>>>();
                    if (result?.Success == true)
                    {
                        return result.Data;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<BackupResponseDto?> UploadBackupAsync(string filePath, string machineId)
        {
            try
            {
                EnsureAuthenticated();
                
                using var form = new MultipartFormDataContent();
                using var fileStream = File.OpenRead(filePath);
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                form.Add(streamContent, "file", Path.GetFileName(filePath));
                form.Add(new StringContent(machineId), "machine_id");

                var response = await _httpClient.PostAsync("backup", form);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<BackupResponseDto>>();
                    if (result?.Success == true)
                    {
                        return result.Data;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
