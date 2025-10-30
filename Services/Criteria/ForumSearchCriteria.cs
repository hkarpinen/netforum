namespace NETForum.Services.Criteria
{
    public class ForumSearchCriteria
    {
        public string? Name {  get; set; }
        public int? CategoryId { get; set; }
        public int? ParentForumId { get; set; }
        public bool? Published { get; set; }
        public string SortBy { get; set; }
        public bool Ascending { get; set; } = true;
    }
}
