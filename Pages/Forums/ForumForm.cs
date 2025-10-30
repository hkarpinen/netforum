using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NETForum.Pages.Forums
{
    public class ForumForm
    {
        public int Id { get; set; }
        public int? ParentForumId { get; set; }
        public int? CategoryId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;
        public bool Published { get; set; }

        public IEnumerable<SelectListItem>? AvailableParentForums { get; set; }
        public IEnumerable<SelectListItem> AvailableCategories = new List<SelectListItem>();
    }
}
