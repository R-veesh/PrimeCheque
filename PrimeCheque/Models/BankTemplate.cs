using System;

namespace PrimeCheque.Models
{
    public class BankTemplate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BankName { get; set; } = string.Empty;
        public string SeriesName { get; set; } = string.Empty;     // e.g. "Current Account Cheque – Series A"
        public string TemplateConfig { get; set; } = "{}";         // JSON: field positions (x, y coordinates in mm)
        public string? TemplateImagePath { get; set; }             // Relative path to cheque background image (e.g. "template_image/BOC_LK.png")
        public decimal ChequeWidthMm { get; set; } = 200m;
        public decimal ChequeHeightMm { get; set; } = 88m;
        public bool IsDefault { get; set; }                        // System-provided template
        public Guid? CompanyId { get; set; }                       // Custom templates per company (nullable)
        public Guid? BankId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string DisplayName => !string.IsNullOrWhiteSpace(SeriesName) ? $"{BankName} – {SeriesName}" : BankName;

        // Navigation properties
        public Bank? Bank { get; set; }
        public Company? Company { get; set; }
    }
}
