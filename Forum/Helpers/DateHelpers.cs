namespace NETForum.Helpers
{
    public static class DateHelpers
    {
        public static string FormatDatetime(DateTime dateTime)
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);
            var dateOnly = dateTime.Date;

            var datePart = dateOnly switch
            {
                var d when d == today => "Today",
                var d when d == yesterday => "Yesterday",
                _ => dateTime.ToShortDateString()
            };

            return $"{datePart} at {dateTime.ToShortTimeString()}";
        }
    }
}
