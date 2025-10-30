using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NETForum.Pages.Users
{
    public class UserForm
    {
        public int Id { get; set; }
        
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Email { get; set; }
        public IEnumerable<string> SelectedRoleNames { get; set; } = new List<string>();
        public IEnumerable<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>();
    }
}
