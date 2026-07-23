using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.ViewModels
{
    public partial class AuditLogViewModel : ObservableObject
    {
        private readonly IAuditService _auditService;

        [ObservableProperty]
        private ObservableCollection<ChequeAuditLog> _auditLogs = new();

        [ObservableProperty]
        private string _userFilter = string.Empty;

        [ObservableProperty]
        private string _actionFilter = string.Empty;

        [ObservableProperty]
        private ChequeAuditLog? _selectedLog;

        public AuditLogViewModel(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task LoadAuditLogsAsync()
        {
            var logs = await _auditService.GetAllAuditLogsAsync(UserFilter, ActionFilter);
            AuditLogs.Clear();
            foreach (var l in logs) AuditLogs.Add(l);
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            await LoadAuditLogsAsync();
        }
    }
}
