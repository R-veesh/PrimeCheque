using System;

namespace PrimeCheque.Models
{
    public class PrinterCalibration
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PrinterName { get; set; } = string.Empty;
        public string? TrayName { get; set; }
        public decimal HorizontalOffsetMm { get; set; }
        public decimal VerticalOffsetMm { get; set; }
        public Guid? TemplateId { get; set; }
        public bool PrintLandscape { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
