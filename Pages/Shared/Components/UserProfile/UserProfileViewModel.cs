namespace NETForum.Pages.Shared.Components.UserProfile
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public IEnumerable<string> RoleNames { get; set; } = new List<string>();
    }
}
