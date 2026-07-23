using System;
using System.Collections.Generic;

namespace PrimeCheque.Models
{
    public enum ChequeBookStatus
    {
        Active,
        Exhausted,
        Cancelled
    }

    public class ChequeBook
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public Guid BankId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;  // Stored encrypted (AES-256)
        public string MaskedAccountNumber { get; set; } = string.Empty;
        public int StartChequeNo { get; set; }
        public int EndChequeNo { get; set; }
        public int CurrentChequeNo { get; set; }
        public ChequeBookStatus Status { get; set; } = ChequeBookStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Company Company { get; set; } = null!;
        public Bank Bank { get; set; } = null!;
        public ICollection<Cheque> Cheques { get; set; } = new List<Cheque>();
    }
}
