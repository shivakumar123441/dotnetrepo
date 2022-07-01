namespace InvestTrackerWebApi.Domain.Identity;

public static class Permissions
{
    public static readonly Permission[] RootAdmin = new Permission[]
    {
        new("View Users", CRUDAction.View, Resource.Users),
        new("Create Users", CRUDAction.Create, Resource.Users),
        new("Update Users", CRUDAction.Update, Resource.Users),
        new("Delete Users", CRUDAction.Delete, Resource.Users),
        new("View UserRoles", UsersOnlyAction.ViewRoles, Resource.Users),
        new("Update UserRoles", UsersOnlyAction.UpdateRoles, Resource.Users),
        new("View Roles", CRUDAction.View, Resource.Roles),
        new("Create Roles", CRUDAction.Create, Resource.Roles),
        new("Update Roles", CRUDAction.Update, Resource.Roles),
        new("Delete Roles", CRUDAction.Delete, Resource.Roles),
        new("View RoleClaims", RolesOnlyAction.ViewRoleClaims, Resource.Roles),
        new("Update RoleClaims", RolesOnlyAction.UpdateRoleClaims, Resource.Roles),
        new("View Accounts", CRUDAction.View, Resource.Accounts),
        new("Create Accounts", CRUDAction.Create, Resource.Accounts),
        new("Update Accounts", CRUDAction.Update, Resource.Accounts),
        new("Delete Accounts", CRUDAction.Delete, Resource.Accounts),
        new("View Transactions", CRUDAction.View, Resource.Transactions),
        new("Create Transactions", CRUDAction.Create, Resource.Transactions),
        new("View Current User Accounts", PersonalOnlyAction.ViewAccounts, Resource.Personal),
        new("View Current User Account Details", PersonalOnlyAction.ViewAccountDetails, Resource.Personal),
        new("View Current User Transactions", PersonalOnlyAction.ViewTransactions, Resource.Personal),
        new("View Current User Transaction Details", PersonalOnlyAction.ViewTransactionDetails, Resource.Personal),
        new("View Current User Details", PersonalOnlyAction.ViewUserDetails, Resource.Personal),
        new("View Current User Permissions", PersonalOnlyAction.ViewPermissions, Resource.Personal),
        new("Update Current User Details", PersonalOnlyAction.UpdateUserDetails, Resource.Personal),
        new("Update Current User Password", PersonalOnlyAction.UpdatePassword, Resource.Personal),
        new("View Current User Logs", PersonalOnlyAction.ViewLogs, Resource.Personal)
    };

    public static readonly Permission[] Admin = new Permission[]
    {
        new("View Users", CRUDAction.View, Resource.Users),
        new("Create Users", CRUDAction.Create, Resource.Users),
        new("Update Users", CRUDAction.Update, Resource.Users),
        new("Delete Users", CRUDAction.Delete, Resource.Users),
        new("View UserRoles", UsersOnlyAction.ViewRoles, Resource.Users),
        new("Update UserRoles", UsersOnlyAction.UpdateRoles, Resource.Users),
        new("View Roles", CRUDAction.View, Resource.Roles),
        new("Create Roles", CRUDAction.Create, Resource.Roles),
        new("Update Roles", CRUDAction.Update, Resource.Roles),
        new("Delete Roles", CRUDAction.Delete, Resource.Roles),
        new("View RoleClaims", RolesOnlyAction.ViewRoleClaims, Resource.Roles),
        new("Update RoleClaims", RolesOnlyAction.UpdateRoleClaims, Resource.Roles),
        new("View Current User Accounts", PersonalOnlyAction.ViewAccounts, Resource.Personal),
        new("View Current User Account Details", PersonalOnlyAction.ViewAccountDetails, Resource.Personal),
        new("View Current User Transactions", PersonalOnlyAction.ViewTransactions, Resource.Personal),
        new("View Current User Transaction Details", PersonalOnlyAction.ViewTransactionDetails, Resource.Personal),
        new("View Current User Details", PersonalOnlyAction.ViewUserDetails, Resource.Personal),
        new("View Current User Permissions", PersonalOnlyAction.ViewPermissions, Resource.Personal),
        new("Update Current User Details", PersonalOnlyAction.UpdateUserDetails, Resource.Personal),
        new("Update Current User Password", PersonalOnlyAction.UpdatePassword, Resource.Personal),
        new("View Current User Logs", PersonalOnlyAction.ViewLogs, Resource.Personal)
    };

    public static readonly Permission[] Manager = new Permission[]
    {
        new("View Accounts", CRUDAction.View, Resource.Accounts),
        new("Create Accounts", CRUDAction.Create, Resource.Accounts),
        new("Update Accounts", CRUDAction.Update, Resource.Accounts),
        new("Delete Accounts", CRUDAction.Delete, Resource.Accounts),
        new("View Transactions", CRUDAction.View, Resource.Transactions),
        new("Create Transactions", CRUDAction.Create, Resource.Transactions),
        new("View Current User Accounts", PersonalOnlyAction.ViewAccounts, Resource.Personal),
        new("View Current User Account Details", PersonalOnlyAction.ViewAccountDetails, Resource.Personal),
        new("View Current User Transactions", PersonalOnlyAction.ViewTransactions, Resource.Personal),
        new("View Current User Transaction Details", PersonalOnlyAction.ViewTransactionDetails, Resource.Personal),
        new("View Current User Details", PersonalOnlyAction.ViewUserDetails, Resource.Personal),
        new("View Current User Permissions", PersonalOnlyAction.ViewPermissions, Resource.Personal),
        new("Update Current User Details", PersonalOnlyAction.UpdateUserDetails, Resource.Personal),
        new("Update Current User Password", PersonalOnlyAction.UpdatePassword, Resource.Personal),
        new("View Current User Logs", PersonalOnlyAction.ViewLogs, Resource.Personal)
    };

    public static readonly Permission[] Basic = new Permission[]
    {
        new("View Current User Accounts", PersonalOnlyAction.ViewAccounts, Resource.Personal),
        new("View Current User Account Details", PersonalOnlyAction.ViewAccountDetails, Resource.Personal),
        new("View Current User Transactions", PersonalOnlyAction.ViewTransactions, Resource.Personal),
        new("View Current User Transaction Details", PersonalOnlyAction.ViewTransactionDetails, Resource.Personal),
        new("View Current User Details", PersonalOnlyAction.ViewUserDetails, Resource.Personal),
        new("View Current User Permissions", PersonalOnlyAction.ViewPermissions, Resource.Personal),
        new("Update Current User Details", PersonalOnlyAction.UpdateUserDetails, Resource.Personal),
        new("Update Current User Password", PersonalOnlyAction.UpdatePassword, Resource.Personal),
        new("View Current User Logs", PersonalOnlyAction.ViewLogs, Resource.Personal)
    };
}
