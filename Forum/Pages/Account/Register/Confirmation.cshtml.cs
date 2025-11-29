using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NETForum.Pages.Account.Register;

public class Confirmation : PageModel
{
    public string Email { get; set; } = string.Empty;
    
    public void OnGet(string email)
    {
        Email = email;
    }
}