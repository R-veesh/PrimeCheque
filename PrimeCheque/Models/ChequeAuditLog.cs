using System;

namespace PrimeCheque.Models
{
    public class ChequeAuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ChequeId { get; set; }
        public string ActionType { get; set; } = string.Empty;   // Created, Approved, Printed, Voided, etc.
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? BeforeState { get; set; }                  // JSON
        public string? AfterState { get; set; }                   // JSON

        // Navigation property
        public Cheque Cheque { get; set; } = null!;
    }
}
