using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NETForum.Models.Entities;

namespace NETForum.Data.Configurations
{
    public class UserProfileEntityTypeConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(up => up.Id);
            builder.Property(up => up.UserId)
                .IsRequired();
            builder.Property(up => up.Bio)
                .HasMaxLength(1000)
                .IsRequired(false);
            builder.Property(up => up.Signature)
                .HasMaxLength(500)
                .IsRequired(false);
            builder.Property(up => up.Location)
                .HasMaxLength(100)
                .IsRequired(false);
            builder.Property(up => up.DateOfBirth)
                .IsRequired(false);
            builder.Property(up => up.LastUpdated);
        }
    }
}
