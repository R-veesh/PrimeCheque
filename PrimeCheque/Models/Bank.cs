using System;
using System.Collections.Generic;

namespace PrimeCheque.Models
{
    public class Bank
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;          // e.g. "Commercial Bank of Ceylon"
        public string? ShortName { get; set; }                     // e.g. "COMBANK"
        public string? BranchCode { get; set; }
        public string? SwiftCode { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ChequeBook> ChequeBooks { get; set; } = new List<ChequeBook>();
        public ICollection<BankTemplate> Templates { get; set; } = new List<BankTemplate>();
    }
}
