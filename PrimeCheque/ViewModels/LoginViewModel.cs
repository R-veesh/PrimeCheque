using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IStaticAuthService _staticAuth;
        private readonly ISessionService _session;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isErrorVisible;

        public Action? OnLoginSucceeded { get; set; }

        public LoginViewModel(IStaticAuthService staticAuth, ISessionService session)
        {
            _staticAuth = staticAuth;
            _session = session;
        }

        [RelayCommand]
        private void Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password.";
                IsErrorVisible = true;
                return;
            }

            var user = _staticAuth.Authenticate(Username.Trim(), Password);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                IsErrorVisible = true;
                return;
            }

            _session.CurrentUser = user;
            IsErrorVisible = false;
            OnLoginSucceeded?.Invoke();
        }
    }
}
