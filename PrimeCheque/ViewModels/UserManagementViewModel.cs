using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IStaticAuthService _authService;
        private readonly ISessionService _sessionService;

        // Profile
        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _lastLogin = string.Empty;

        // Change Password
        [ObservableProperty]
        private string _currentPassword = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        // Security Question
        [ObservableProperty]
        private string _securityQuestion = string.Empty;

        [ObservableProperty]
        private string _securityAnswer = string.Empty;

        [ObservableProperty]
        private bool _hasSecurityQuestion;

        // Status messages
        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private string _passwordStatusMessage = string.Empty;

        [ObservableProperty]
        private string _securityStatusMessage = string.Empty;

        public List<string> SecurityQuestions { get; } = new()
        {
            "What is your mother's maiden name?",
            "What was the name of your first pet?",
            "What city were you born in?",
            "What is your favorite book?",
            "What was your childhood nickname?",
            "What is the name of your first school?"
        };

        public UserManagementViewModel(IUserService userService, IStaticAuthService authService, ISessionService sessionService)
        {
            _userService = userService;
            _authService = authService;
            _sessionService = sessionService;
        }

        public async Task LoadProfileAsync()
        {
            var admin = await _userService.GetAdminAsync();
            if (admin != null)
            {
                DisplayName = admin.DisplayName;
                Username = admin.Username;
                LastLogin = admin.LastLoginAt?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "Never";
                HasSecurityQuestion = !string.IsNullOrEmpty(admin.SecurityQuestion);
                if (HasSecurityQuestion)
                {
                    SecurityQuestion = admin.SecurityQuestion!;
                }
            }
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                StatusMessage = "Display name cannot be empty.";
                return;
            }

            try
            {
                await _userService.UpdateAdminProfileAsync(DisplayName.Trim());

                // Update session
                if (_sessionService.CurrentUser != null)
                {
                    _sessionService.CurrentUser.DisplayName = DisplayName.Trim();
                }

                StatusMessage = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                PasswordStatusMessage = "Please fill in all password fields.";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                PasswordStatusMessage = "New passwords do not match.";
                return;
            }

            if (NewPassword.Length < 6)
            {
                PasswordStatusMessage = "Password must be at least 6 characters.";
                return;
            }

            try
            {
                var userId = _sessionService.CurrentUser?.Id ?? Guid.Empty;
                var changed = await _authService.ChangePasswordAsync(userId, CurrentPassword, NewPassword);

                if (changed)
                {
                    PasswordStatusMessage = "Password changed successfully!";
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                }
                else
                {
                    PasswordStatusMessage = "Current password is incorrect.";
                }
            }
            catch (Exception ex)
            {
                PasswordStatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task SaveSecurityQuestionAsync()
        {
            if (string.IsNullOrWhiteSpace(SecurityQuestion) || string.IsNullOrWhiteSpace(SecurityAnswer))
            {
                SecurityStatusMessage = "Please select a security question and provide an answer.";
                return;
            }

            try
            {
                var userId = _sessionService.CurrentUser?.Id ?? Guid.Empty;
                var saved = await _authService.SetSecurityQuestionAsync(userId, SecurityQuestion, SecurityAnswer);

                if (saved)
                {
                    SecurityStatusMessage = "Security question saved successfully!";
                    HasSecurityQuestion = true;
                    SecurityAnswer = string.Empty;
                }
                else
                {
                    SecurityStatusMessage = "Failed to save security question.";
                }
            }
            catch (Exception ex)
            {
                SecurityStatusMessage = $"Error: {ex.Message}";
            }
        }
    }
}
