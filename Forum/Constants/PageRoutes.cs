namespace NETForum.Constants;

public static class PageRoutes
{
    // Account
    public const string Login = "/Account/Login";
    public const string Logout = "/Account/Logout";
    public const string ProfileEdit = "/Account/Profile/Edit";
    public const string Register = "/Account/Register";
    public const string RegisterConfirmation = "/Account/Register/Confirmation";
    public const string ConfirmEmail = "/Account/ConfirmEmail";
    
    // Admin
    public const string ManageForums = "/Admin/Forums";
    public const string CreateForum = "/Admin/Forums/Create";
    public const string EditForum = "/Admin/Forums/Edit";
    public const string DeleteForum = "/Admin/Forums/Delete";
    
    public const string ManageCategories = "/Admin/Categories";
    public const string CreateCategory = "/Admin/Categories/Create";
    public const string EditCategory = "/Admin/Categories/Edit";
    public const string DeleteCategory = "/Admin/Categories/Delete";
    
    public const string ManageMembers = "/Admin/Members/Manage";
    public const string CreateMember = "/Admin/Members/Create";
    public const string EditMember = "/Admin/Members/Edit";
    public const string DeleteMember = "/Admin/Members/Delete"; 
    
    public const string ManagePosts = "/Admin/Posts/Manage";
    public const string DeletePost = "/Admin/Posts/Delete";
    
    public const string ManageRoles  = "/Admin/Roles/Manage";
    public const string CreateRole = "/Admin/Roles/Create";
    public const string EditRole = "/Admin/Roles/Edit";
    public const string DeleteRole = "/Admin/Roles/Delete";
    
    // Forums
    public const string ForumView = "/Forums/View";
    public const string ForumLanding = "/Forums/Index";
    
    // Install
    public const string Install = "/Install";
    
    // Members
    public const string MembersLanding = "/Members/Index";
    public const string MemberView = "/Members/View";
    
    // Posts
    public const string PostsLanding = "/Posts/Index";
    public const string PostView = "/Posts/View";
    public const string PostCreate = "/Posts/Create"; 
    public const string PostEdit = "/Posts/Edit";
}