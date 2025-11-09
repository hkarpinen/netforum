namespace NETForum.Repositories.Filters
{
    public class ForumFilterOptions
    {
        public string? Name {  get; set; }
        public int? CategoryId { get; set; }
        public int? ParentForumId { get; set; }
        public bool? Published { get; set; }
    }
}
