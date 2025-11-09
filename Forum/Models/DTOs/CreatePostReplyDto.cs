using System.ComponentModel.DataAnnotations;

namespace NETForum.Models.DTOs
{
    public class CreatePostReplyDto
    {
        [Required]
        [StringLength(10000, MinimumLength = 10)]
        public string Content { get; set; } = string.Empty;
    }
}
