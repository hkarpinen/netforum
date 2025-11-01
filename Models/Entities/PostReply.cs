namespace NETForum.Models
{
    public class PostReply
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public Post? Post { get; set; }
        public User? Author { get; set; }
    }
}
