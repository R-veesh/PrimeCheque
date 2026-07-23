using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class AuditService : IAuditService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public AuditService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogEventAsync(Guid chequeId, string actionType, string performedBy, object? beforeState = null, object? afterState = null)
        {
            var auditLog = new ChequeAuditLog
            {
                Id = Guid.NewGuid(),
                ChequeId = chequeId,
                ActionType = actionType,
                PerformedBy = performedBy,
                Timestamp = DateTime.UtcNow,
                BeforeState = beforeState != null ? JsonSerializer.Serialize(beforeState) : null,
                AfterState = afterState != null ? JsonSerializer.Serialize(afterState) : null
            };

            _dbContext.ChequeAuditLogs.Add(auditLog);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ChequeAuditLog>> GetAuditLogsForChequeAsync(Guid chequeId)
        {
            return await _dbContext.ChequeAuditLogs
                .Where(al => al.ChequeId == chequeId)
                .OrderByDescending(al => al.Timestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ChequeAuditLog>> GetAllAuditLogsAsync(string? userFilter = null, string? actionFilter = null)
        {
            var query = _dbContext.ChequeAuditLogs.Include(al => al.Cheque).AsQueryable();

            if (!string.IsNullOrWhiteSpace(userFilter))
            {
                query = query.Where(al => al.PerformedBy.Contains(userFilter));
            }

            if (!string.IsNullOrWhiteSpace(actionFilter))
            {
                query = query.Where(al => al.ActionType.Contains(actionFilter));
            }

            return await query.OrderByDescending(al => al.Timestamp).AsNoTracking().ToListAsync();
        }
    }
}
