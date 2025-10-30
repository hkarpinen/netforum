using NETForum.Pages.Roles;

namespace NETForum.Extensions;

public static class RoleExtension
{
    public static RoleForm ToForm(this Models.Role role)
    {
        return new RoleForm
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description
        };
    }
}