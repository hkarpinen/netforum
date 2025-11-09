namespace NETForum.Helpers
{
    public static class UserHelpers
    {
        public static string GetInitials(string userName)
        {
            var firstLetter = userName.First();
            var lastLetter = userName.Last();
            return $"{char.ToUpper(firstLetter)}{char.ToUpper(lastLetter)}";
        }
    }
}
