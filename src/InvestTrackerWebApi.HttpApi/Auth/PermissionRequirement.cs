namespace InvestTrackerWebApi.HttpApi.Auth;
using Microsoft.AspNetCore.Authorization;

internal class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; private set; }

    public PermissionRequirement(string permission) =>
        this.Permission = permission;
}
