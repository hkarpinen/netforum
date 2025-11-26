namespace NETForum.Models.Entities
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
        
        // This is really not needed since we have the replies navigation.
        // TODO: This should be removed and any references, updated to count Replies.
        public int ReplyCount { get; set; }

        // Navigation properties
        public User? Author { get; set; }
        public Forum? Forum { get; set; }
        public ICollection<Reply> Replies { get; set; } = new List<Reply>();
    }
}
