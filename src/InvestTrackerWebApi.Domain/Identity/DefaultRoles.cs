namespace InvestTrackerWebApi.Domain.Identity;

public static class DefaultRoles
{
    public const string RootAdmin = RootConstants.RootAdminRole;
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Basic = "Basic";

    public static List<string> Get() => new()
    {
        RootAdmin,
        Admin,
        Manager,
        Basic
    };
}
