using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogEventAsync(Guid chequeId, string actionType, string performedBy, object? beforeState = null, object? afterState = null);
        Task<List<ChequeAuditLog>> GetAuditLogsForChequeAsync(Guid chequeId);
        Task<List<ChequeAuditLog>> GetAllAuditLogsAsync(string? userFilter = null, string? actionFilter = null);
    }
}
