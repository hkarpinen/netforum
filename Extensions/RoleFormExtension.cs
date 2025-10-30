using NETForum.Pages.Roles;

namespace NETForum.Extensions;

public static class RoleFormExtension
{
    public static Models.Role ToNewRole(this RoleForm roleForm)
    {
        return new Models.Role
        {
            Name = roleForm.Name,
            Description = roleForm.Description,
            NormalizedName = roleForm.Name
        };
    }

    public static Models.Role MapToRole(this RoleForm roleForm, Models.Role role)
    {
        role.Name = roleForm.Name;
        role.Description = roleForm.Description;
        return role;
    }
}