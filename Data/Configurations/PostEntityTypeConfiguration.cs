using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NETForum.Models;

namespace NETForum.Data.Configurations
{
    public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.ForumId).IsRequired();
            builder.Property(t => t.AuthorId).IsRequired();
            builder.Property(t => t.IsPinned).IsRequired();
            builder.Property(t => t.IsLocked).IsRequired();
            builder.Property(t => t.Published).IsRequired();
            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(t => t.Content)
                .HasMaxLength(10000)
                .IsRequired();
            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.UpdatedAt).IsRequired();
            builder.Property(t => t.ViewCount).IsRequired();
            builder.Property(t => t.ReplyCount).IsRequired();

            // 1. Post -> User (Author) relationship
            builder.HasOne(p => p.Author)
                .WithMany() // User doesn't need navigation back to Posts
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
