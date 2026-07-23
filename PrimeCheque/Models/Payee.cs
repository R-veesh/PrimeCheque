using System;

namespace PrimeCheque.Models
{
    public class Payee
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public string? DefaultMemo { get; set; }
        public decimal? LastAmount { get; set; }
        public bool IsFavourite { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Company Company { get; set; } = null!;
    }
}
