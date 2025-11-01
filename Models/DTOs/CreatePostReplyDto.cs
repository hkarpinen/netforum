using System.ComponentModel.DataAnnotations;

namespace NETForum.Pages.Posts
{
    public class CreatePostReplyDto
    {
        [Required]
        [StringLength(10000, MinimumLength = 10)]
        public string Content { get; set; } = string.Empty;
    }
}
