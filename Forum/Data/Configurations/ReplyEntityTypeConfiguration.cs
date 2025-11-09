using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NETForum.Models.Entities;

namespace NETForum.Data.Configurations
{
    public class ReplyEntityTypeConfiguration : IEntityTypeConfiguration<Reply>
    {
        public void Configure(EntityTypeBuilder<Reply> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.PostId)
                .IsRequired();
            builder.Property(p => p.AuthorId)
                .IsRequired();
            builder.Property(p => p.Content)
                .HasMaxLength(1000)
                .IsRequired();
            builder.Property(p => p.CreatedAt);
            builder.Property(p => p.LastUpdatedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            // 1. Reply -> Post relationship
            builder.HasOne(r => r.Post)
                .WithMany(p => p.Replies)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Reply -> User (Author) relationship
            builder.HasOne(r => r.Author)
                .WithMany() // User doesn't need navigation back to Replies
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
