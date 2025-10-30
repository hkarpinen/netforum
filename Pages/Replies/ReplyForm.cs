using System.ComponentModel.DataAnnotations;

namespace NETForum.Pages.Replies
{
    public class ReplyForm
    {
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public int AuthorId { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 4)]
        public string AuthorName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(10000, MinimumLength = 10)]
        public string Content { get; set; } = string.Empty;
    }
}
