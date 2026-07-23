using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.RegistrationNumber).HasMaxLength(100);
            builder.Property(c => c.Address).HasMaxLength(500);
            builder.Property(c => c.Phone).HasMaxLength(50);
            builder.Property(c => c.Email).HasMaxLength(100);
        }
    }
}
