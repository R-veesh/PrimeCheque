using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Configurations
{
    public class ChequeBookConfiguration : IEntityTypeConfiguration<ChequeBook>
    {
        public void Configure(EntityTypeBuilder<ChequeBook> builder)
        {
            builder.HasKey(cb => cb.Id);
            builder.Property(cb => cb.AccountNumber).IsRequired().HasMaxLength(256);
            builder.Property(cb => cb.MaskedAccountNumber).HasMaxLength(50);
            builder.Property(cb => cb.Status).HasConversion<string>();

            builder.HasOne(cb => cb.Company)
                   .WithMany(c => c.ChequeBooks)
                   .HasForeignKey(cb => cb.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cb => cb.Bank)
                   .WithMany(b => b.ChequeBooks)
                   .HasForeignKey(cb => cb.BankId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
