using System.ComponentModel.DataAnnotations;

namespace NETForum.Pages.Account.Login
{
    public class LoginFormModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

    }
}
