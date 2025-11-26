using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NETForum.Models.Entities;

namespace NETForum.Data.Configurations
{
    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(r => r.Description)
                .HasMaxLength(250)
                .IsRequired();

            // Build Indexes for faster searches
            builder.HasIndex(r => r.Name).IsUnique();
        }
    }
}
