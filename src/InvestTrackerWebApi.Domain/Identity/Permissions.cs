namespace InvestTrackerWebApi.Domain.Identity;

public static class CRUDAction
{
    public const string View = nameof(View);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
}

public static class CommmonAction
{
    public const string Export = nameof(Export);
}

public static class UsersOnlyAction
{
    public const string ViewRoles = nameof(ViewRoles);
    public const string UpdateRoles = nameof(UpdateRoles);
    public const string ToggleUserStatus = nameof(ToggleUserStatus);
}

public static class RolesOnlyAction
{
    public const string ViewRoleClaims = nameof(ViewRoleClaims);
    public const string UpdateRoleClaims = nameof(UpdateRoleClaims);
}

public static class PersonalOnlyAction
{
    public const string ViewAccounts = nameof(ViewAccounts);
    public const string ViewAccountDetails = nameof(ViewAccountDetails);
    public const string ViewTransactions = nameof(ViewTransactions);
    public const string ViewTransactionDetails = nameof(ViewTransactionDetails);
    public const string ViewUserDetails = nameof(ViewUserDetails);
    public const string ViewPermissions = nameof(ViewPermissions);
    public const string UpdateUserDetails = nameof(UpdateUserDetails);
    public const string UpdatePassword = nameof(UpdatePassword);
    public const string ViewLogs = nameof(ViewLogs);
}

public static class Resource
{
    public const string Users = nameof(Users);
    public const string Roles = nameof(Roles);
    public const string Accounts = nameof(Accounts);
    public const string Transactions = nameof(Transactions);
    public const string Audits = nameof(Audits);
    public const string Personal = nameof(Personal);
}

public record Permission(string Description, string Action, string Resource)
{
    public string Name => NameFor(this.Action, this.Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}
