using System;
using System.Collections.ObjectModel;
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

        [ObservableProperty]
        private ObservableCollection<User> _users = new();

        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private UserRole _selectedRole = UserRole.ChequePreparer;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        public Array AvailableRoles => Enum.GetValues(typeof(UserRole));

        public UserManagementViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task LoadUsersAsync()
        {
            var list = await _userService.GetAllUsersAsync();
            Users.Clear();
            foreach (var u in list) Users.Add(u);
        }

        [RelayCommand]
        private async Task CreateUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(DisplayName))
            {
                StatusMessage = "Username and Display Name are required.";
                return;
            }

            var newUser = new User
            {
                Username = Username.Trim(),
                DisplayName = DisplayName.Trim(),
                Role = SelectedRole,
                IsActive = true
            };

            await _userService.CreateUserAsync(newUser, "default123");
            StatusMessage = $"User '{Username}' created successfully with role '{SelectedRole}'.";

            Username = string.Empty;
            DisplayName = string.Empty;
            await LoadUsersAsync();
        }

        [RelayCommand]
        private async Task ToggleUserActiveAsync()
        {
            if (SelectedUser == null) return;

            SelectedUser.IsActive = !SelectedUser.IsActive;
            await _userService.UpdateUserAsync(SelectedUser);
            StatusMessage = $"User '{SelectedUser.Username}' active status set to {SelectedUser.IsActive}.";
            await LoadUsersAsync();
        }
    }
}
