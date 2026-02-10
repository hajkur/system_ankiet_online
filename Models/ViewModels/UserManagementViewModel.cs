namespace zad1.Models.ViewModels;

public class UserManagementViewModel
{
    public List<UserWithRolesViewModel> Users { get; set; } = new();
    public List<string> AvailableRoles { get; set; } = new();
}

public class UserWithRolesViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public class ManageUserRolesViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<RoleCheckboxViewModel> Roles { get; set; } = new();
}

public class RoleCheckboxViewModel
{
    public string RoleName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}
