using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IStaticAuthService _authService;
        private readonly ISessionService _session;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isErrorVisible;

        // Password Reset properties
        [ObservableProperty]
        private bool _isForgotPasswordVisible;

        [ObservableProperty]
        private string _resetUsername = string.Empty;

        [ObservableProperty]
        private string _securityQuestion = string.Empty;

        [ObservableProperty]
        private string _securityAnswer = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _isSecurityQuestionVisible;

        [ObservableProperty]
        private bool _isNewPasswordVisible;

        [ObservableProperty]
        private string _resetMessage = string.Empty;

        [ObservableProperty]
        private bool _isResetMessageVisible;

        // First-login Change Password properties
        [ObservableProperty]
        private bool _isMustChangePasswordVisible;

        [ObservableProperty]
        private string _firstLoginNewPassword = string.Empty;

        [ObservableProperty]
        private string _firstLoginConfirmPassword = string.Empty;

        [ObservableProperty]
        private string _firstLoginSecurityQuestion = string.Empty;

        [ObservableProperty]
        private string _firstLoginSecurityAnswer = string.Empty;

        [ObservableProperty]
        private string _changePasswordMessage = string.Empty;

        [ObservableProperty]
        private bool _isChangePasswordMessageVisible;

        public List<string> SecurityQuestions { get; } = new()
        {
            "What is your mother's maiden name?",
            "What was the name of your first pet?",
            "What city were you born in?",
            "What is your favorite book?",
            "What was your childhood nickname?",
            "What is the name of your first school?"
        };

        public Action? OnLoginSucceeded { get; set; }

        public LoginViewModel(IStaticAuthService authService, ISessionService session)
        {
            _authService = authService;
            _session = session;
        }

        [RelayCommand]
        private async void Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password.";
                IsErrorVisible = true;
                return;
            }

            try
            {
                var user = await _authService.AuthenticateAsync(Username.Trim(), Password);

                if (user == null)
                {
                    ErrorMessage = "Invalid username or password. Account locks after 5 failed attempts.";
                    IsErrorVisible = true;
                    return;
                }

                _session.CurrentUser = user;
                IsErrorVisible = false;

                // Check if user must change password on first login
                if (user.MustChangePassword)
                {
                    IsMustChangePasswordVisible = true;
                    return;
                }

                OnLoginSucceeded?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login error: {ex.Message}";
                IsErrorVisible = true;
            }
        }

        [RelayCommand]
        private async void CompleteFirstLogin()
        {
            if (string.IsNullOrWhiteSpace(FirstLoginNewPassword) || string.IsNullOrWhiteSpace(FirstLoginConfirmPassword))
            {
                ChangePasswordMessage = "Please fill in all fields.";
                IsChangePasswordMessageVisible = true;
                return;
            }

            if (FirstLoginNewPassword != FirstLoginConfirmPassword)
            {
                ChangePasswordMessage = "Passwords do not match.";
                IsChangePasswordMessageVisible = true;
                return;
            }

            if (FirstLoginNewPassword.Length < 6)
            {
                ChangePasswordMessage = "Password must be at least 6 characters.";
                IsChangePasswordMessageVisible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(FirstLoginSecurityQuestion) || string.IsNullOrWhiteSpace(FirstLoginSecurityAnswer))
            {
                ChangePasswordMessage = "Please set a security question and answer for password recovery.";
                IsChangePasswordMessageVisible = true;
                return;
            }

            try
            {
                var user = _session.CurrentUser;
                if (user == null) return;

                var changed = await _authService.ChangePasswordAsync(user.Id, Password, FirstLoginNewPassword);
                if (!changed)
                {
                    ChangePasswordMessage = "Failed to change password.";
                    IsChangePasswordMessageVisible = true;
                    return;
                }

                await _authService.SetSecurityQuestionAsync(user.Id, FirstLoginSecurityQuestion, FirstLoginSecurityAnswer);

                IsMustChangePasswordVisible = false;
                IsChangePasswordMessageVisible = false;
                OnLoginSucceeded?.Invoke();
            }
            catch (Exception ex)
            {
                ChangePasswordMessage = $"Error: {ex.Message}";
                IsChangePasswordMessageVisible = true;
            }
        }

        [RelayCommand]
        private void ShowForgotPassword()
        {
            IsForgotPasswordVisible = true;
            IsSecurityQuestionVisible = false;
            IsNewPasswordVisible = false;
            IsResetMessageVisible = false;
            ResetUsername = string.Empty;
            SecurityAnswer = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        [RelayCommand]
        private void CancelForgotPassword()
        {
            IsForgotPasswordVisible = false;
            IsSecurityQuestionVisible = false;
            IsNewPasswordVisible = false;
            IsResetMessageVisible = false;
        }

        [RelayCommand]
        private async void GetSecurityQuestion()
        {
            if (string.IsNullOrWhiteSpace(ResetUsername))
            {
                ResetMessage = "Please enter your username.";
                IsResetMessageVisible = true;
                return;
            }

            try
            {
                var question = await _authService.GetSecurityQuestionAsync(ResetUsername.Trim());
                if (string.IsNullOrEmpty(question))
                {
                    ResetMessage = "No security question set for this account. Contact system administrator.";
                    IsResetMessageVisible = true;
                    return;
                }

                SecurityQuestion = question;
                IsSecurityQuestionVisible = true;
                IsResetMessageVisible = false;
            }
            catch (Exception ex)
            {
                ResetMessage = $"Error: {ex.Message}";
                IsResetMessageVisible = true;
            }
        }

        [RelayCommand]
        private async void ValidateSecurityAnswer()
        {
            if (string.IsNullOrWhiteSpace(SecurityAnswer))
            {
                ResetMessage = "Please enter your security answer.";
                IsResetMessageVisible = true;
                return;
            }

            try
            {
                var valid = await _authService.ValidateSecurityAnswerAsync(ResetUsername.Trim(), SecurityAnswer);
                if (!valid)
                {
                    ResetMessage = "Incorrect security answer. Please try again.";
                    IsResetMessageVisible = true;
                    return;
                }

                IsNewPasswordVisible = true;
                IsResetMessageVisible = false;
            }
            catch (Exception ex)
            {
                ResetMessage = $"Error: {ex.Message}";
                IsResetMessageVisible = true;
            }
        }

        [RelayCommand]
        private async void ResetPassword()
        {
            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ResetMessage = "Please fill in both password fields.";
                IsResetMessageVisible = true;
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                ResetMessage = "Passwords do not match.";
                IsResetMessageVisible = true;
                return;
            }

            if (NewPassword.Length < 6)
            {
                ResetMessage = "Password must be at least 6 characters.";
                IsResetMessageVisible = true;
                return;
            }

            try
            {
                var success = await _authService.ResetPasswordAsync(ResetUsername.Trim(), NewPassword);
                if (success)
                {
                    ResetMessage = "Password reset successfully! Please login with your new password.";
                    IsResetMessageVisible = true;

                    // Return to login after a moment
                    IsForgotPasswordVisible = false;
                    IsSecurityQuestionVisible = false;
                    IsNewPasswordVisible = false;

                    ErrorMessage = "Password reset successful. Please login with your new password.";
                    IsErrorVisible = false; // We use ErrorMessage style differently here
                }
                else
                {
                    ResetMessage = "Failed to reset password. Please try again.";
                    IsResetMessageVisible = true;
                }
            }
            catch (Exception ex)
            {
                ResetMessage = $"Error: {ex.Message}";
                IsResetMessageVisible = true;
            }
        }
    }
}
