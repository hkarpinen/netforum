using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NETForum.Models;

namespace NETForum.Data.Configurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.UserName)
                .HasMaxLength(30)
                .IsRequired();
            builder.Property(u => u.Email)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(u => u.PasswordHash);
            builder.Property(u => u.CreatedAt);
            builder.Property(u => u.UpdatedAt);
            builder.Property(u => u.ProfileImageUrl);

            // Build Indexes for faster searches
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();

            // Relationship to UserProfile
            builder.HasOne(u => u.UserProfile)
                .WithOne()
                .HasForeignKey<UserProfile>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
