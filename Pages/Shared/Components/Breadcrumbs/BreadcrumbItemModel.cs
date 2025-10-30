namespace NETForum.Pages.Shared.Components.Breadcrumbs
{
    public class BreadcrumbItemModel
    {
        public required string Text { get; set; }
        public required string Url { get; set; }
        public bool Active { get; set; }
    }
}
