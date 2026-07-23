using System;
using System.Collections.Generic;

namespace PrimeCheque.Models
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? RegistrationNumber { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<ChequeBook> ChequeBooks { get; set; } = new List<ChequeBook>();
        public ICollection<Cheque> Cheques { get; set; } = new List<Cheque>();
        public ICollection<Payee> Payees { get; set; } = new List<Payee>();
    }
}
