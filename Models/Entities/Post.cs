namespace NETForum.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int AuthorId { get; set; }
        public bool IsPinned { get; set; }
        public bool IsLocked { get; set; }
        public bool Published { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public int ReplyCount { get; set; }

        // Navigation properties
        public User? Author { get; set; }
        public IEnumerable<PostReply> Replies { get; set; } = new List<PostReply>();
    }
}
