namespace InvestTrackerWebApi.HttpApi.Auth;

using InvestTrackerWebApi.Domain.Identity;
using Microsoft.AspNetCore.Authorization;

public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource) =>
        this.Policy = Permission.NameFor(action, resource);
}
