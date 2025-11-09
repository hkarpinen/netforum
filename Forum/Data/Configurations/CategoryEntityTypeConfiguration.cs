using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NETForum.Models.Entities;

namespace NETForum.Data.Configurations
{
    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(f => f.Description)
                .HasMaxLength(500);
            builder.Property(f => f.SortOrder);
            builder.Property(f => f.Published)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(f => f.CreatedAt);
            builder.Property(f => f.UpdatedAt);
        }
    }
}
