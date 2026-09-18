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
        public string PrintPosition { get; set; } = "Top"; // Used for Vertical (Top, Middle, Bottom)
        public string PrintPositionHorizontal { get; set; } = "Left"; // Used for Horizontal (Left, Center, Right)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
