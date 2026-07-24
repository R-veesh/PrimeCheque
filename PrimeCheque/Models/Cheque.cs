using System;
using System.Collections.Generic;

namespace PrimeCheque.Models
{
    public enum ChequeStatus
    {
        Draft,
        PendingApproval,
        Approved,
        Rejected,
        Printed,
        Void,
        StopPayment
    }

    public enum CrossingType
    {
        None,
        AccountPayeeOnly,
        NotNegotiable,
        AccountPayeeAndNotNegotiable
    }

    public class Cheque
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public Guid ChequeBookId { get; set; }
        public int ChequeNumber { get; set; }
        public string PayeeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AmountInWords { get; set; } = string.Empty;
        public DateOnly ChequeDate { get; set; }
        public string? Memo { get; set; }
        public ChequeStatus Status { get; set; } = ChequeStatus.Draft;
        public CrossingType CrossingType { get; set; } = CrossingType.None;
        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? PrintedAt { get; set; }
        public string? PdfPath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Company Company { get; set; } = null!;
        public ChequeBook ChequeBook { get; set; } = null!;
        public ICollection<ChequeAuditLog> AuditLogs { get; set; } = new List<ChequeAuditLog>();
    }
}
