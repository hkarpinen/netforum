using System.ComponentModel.DataAnnotations;

namespace NETForum.Pages.Posts
{
    public class PostForm
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int AuthorId { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 4)]
        public string AuthorName { get; set; } = string.Empty;
        public bool IsPinned { get; set; }
        public bool IsLocked { get; set; }
        
        [Required]
        public bool Published { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(10000, MinimumLength = 10)]
        public string Content { get; set; } = string.Empty;
    }
}
