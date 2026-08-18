using System;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ISessionService _session;
        private readonly System.Net.Http.HttpClient _httpClient;

        [ObservableProperty]
        private string _companyCode = string.Empty;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isErrorVisible;

        public Action? OnLoginSucceeded { get; set; }

        public LoginViewModel(ISessionService session, System.Net.Http.HttpClient httpClient)
        {
            _session = session;
            _httpClient = httpClient;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(CompanyCode) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter company code, email and password.";
                IsErrorVisible = true;
                return;
            }

            try
            {
                var payload = new 
                {
                    company_code = CompanyCode.Trim(),
                    email = Username.Trim(),
                    password = Password
                };
                
                var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:8000/api/v1/auth/login", payload);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && result.User != null)
                    {
                        var role = Enum.TryParse<PrimeCheque.Models.UserRole>(result.User.Role, out var parsedRole) ? parsedRole : PrimeCheque.Models.UserRole.ChequePreparer;
                        _session.CurrentUser = new PrimeCheque.Models.User
                        {
                            Username = result.User.Email,
                            DisplayName = result.User.Name,
                            Role = role
                        };
                        
                        // Set the token for future API calls
                        // In a real app we'd configure the ApiIntegrationService here
                        
                        IsErrorVisible = false;
                        OnLoginSucceeded?.Invoke();
                        return;
                    }
                }
                
                var errorResult = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                if (errorResult.TryGetProperty("message", out var msg))
                {
                    ErrorMessage = msg.GetString() ?? "Invalid login.";
                }
                else
                {
                    ErrorMessage = "Invalid login credentials.";
                }
                IsErrorVisible = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Network error: " + ex.Message;
                IsErrorVisible = true;
            }
        }
        
        private class LoginResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }
            
            [System.Text.Json.Serialization.JsonPropertyName("user")]
            public LoginUser? User { get; set; }
        }
        
        private class LoginUser
        {
            [System.Text.Json.Serialization.JsonPropertyName("id")]
            public int Id { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            [System.Text.Json.Serialization.JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;
            [System.Text.Json.Serialization.JsonPropertyName("role")]
            public string? Role { get; set; }
        }
    }
}
