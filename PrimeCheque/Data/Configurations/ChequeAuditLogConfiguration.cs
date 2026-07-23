using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Configurations
{
    public class ChequeAuditLogConfiguration : IEntityTypeConfiguration<ChequeAuditLog>
    {
        public void Configure(EntityTypeBuilder<ChequeAuditLog> builder)
        {
            builder.HasKey(al => al.Id);
            builder.Property(al => al.ActionType).IsRequired().HasMaxLength(100);
            builder.Property(al => al.PerformedBy).IsRequired().HasMaxLength(100);

            builder.HasOne(al => al.Cheque)
                   .WithMany(c => c.AuditLogs)
                   .HasForeignKey(al => al.ChequeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
