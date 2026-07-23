using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Configurations
{
    public class BankTemplateConfiguration : IEntityTypeConfiguration<BankTemplate>
    {
        public void Configure(EntityTypeBuilder<BankTemplate> builder)
        {
            builder.HasKey(bt => bt.Id);
            builder.Property(bt => bt.BankName).IsRequired().HasMaxLength(200);
            builder.Property(bt => bt.SeriesName).IsRequired().HasMaxLength(200);
            builder.Property(bt => bt.ChequeWidthMm).HasPrecision(8, 2);
            builder.Property(bt => bt.ChequeHeightMm).HasPrecision(8, 2);

            builder.HasOne(bt => bt.Bank)
                   .WithMany(b => b.Templates)
                   .HasForeignKey(bt => bt.BankId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(bt => bt.Company)
                   .WithMany()
                   .HasForeignKey(bt => bt.CompanyId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
