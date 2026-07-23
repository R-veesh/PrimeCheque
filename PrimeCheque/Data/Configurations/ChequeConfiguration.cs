using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Configurations
{
    public class ChequeConfiguration : IEntityTypeConfiguration<Cheque>
    {
        public void Configure(EntityTypeBuilder<Cheque> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.PayeeName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Amount).HasPrecision(18, 2);
            builder.Property(c => c.AmountInWords).IsRequired().HasMaxLength(500);
            builder.Property(c => c.Memo).HasMaxLength(250);
            builder.Property(c => c.Status).HasConversion<string>();
            builder.Property(c => c.CrossingType).HasConversion<string>();

            builder.HasOne(c => c.Company)
                   .WithMany(comp => comp.Cheques)
                   .HasForeignKey(c => c.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ChequeBook)
                   .WithMany(cb => cb.Cheques)
                   .HasForeignKey(c => c.ChequeBookId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => new { c.ChequeBookId, c.ChequeNumber }).IsUnique();
        }
    }
}
