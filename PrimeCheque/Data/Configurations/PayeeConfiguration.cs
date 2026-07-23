using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Configurations
{
    public class PayeeConfiguration : IEntityTypeConfiguration<Payee>
    {
        public void Configure(EntityTypeBuilder<Payee> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.NickName).HasMaxLength(100);
            builder.Property(p => p.DefaultMemo).HasMaxLength(250);
            builder.Property(p => p.LastAmount).HasPrecision(18, 2);

            builder.HasOne(p => p.Company)
                   .WithMany(c => c.Payees)
                   .HasForeignKey(p => p.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
