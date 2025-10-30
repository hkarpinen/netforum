namespace NETForum.Models
{
    public class Forum
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool Published { get; set; }
        public int? ParentForumId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}

        // Navigation properties
        public Forum? ParentForum { get; set; }
        public Category? Category { get; set; }
        public List<Forum> SubForums { get; set; } = new();
        public List<Post> Posts { get; set; } = new();
    }
}
