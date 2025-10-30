using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NETForum.Models;

namespace NETForum.Data.Configurations
{
    public class ForumEntityTypeConfiguration : IEntityTypeConfiguration<Forum>
    {
        public void Configure(EntityTypeBuilder<Forum> builder)
        {
            builder.HasKey(f => f.Id);

            builder.HasIndex(f => f.Name)
                .IsUnique()
                .HasDatabaseName("IX_Forums_Name");

            builder.Property(f => f.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(f => f.Description)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(f => f.CreatedAt)
                .IsRequired();

            builder.Property(f => f.UpdatedAt)
                .IsRequired();


            // 1. Self-referencing relationship: Forum -> ParentForum
            builder.HasOne(f => f.ParentForum)
                .WithMany(f => f.SubForums)
                .HasForeignKey(f => f.ParentForumId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // 2. Forum -> Category relationship
            builder.HasOne(f => f.Category)
                .WithMany(c => c.Forums) // Assuming Category doesn't need navigation back to Forums
                .HasForeignKey(f => f.CategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // 3. Forum -> Posts relationship
            builder.HasMany(f => f.Posts)
                .WithOne() // We'll define the Post side later
                .HasForeignKey(p => p.ForumId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
