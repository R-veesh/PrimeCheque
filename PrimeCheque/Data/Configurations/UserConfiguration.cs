using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
            builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
            builder.Property(u => u.Role).HasConversion<string>();

            // Security & Password Reset fields
            builder.Property(u => u.SecurityQuestion).HasMaxLength(300);
            builder.Property(u => u.SecurityAnswerHash).HasMaxLength(256);
            builder.Property(u => u.MustChangePassword).HasDefaultValue(true);

            builder.HasIndex(u => u.Username).IsUnique();
        }
    }
}
