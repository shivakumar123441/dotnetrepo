namespace InvestTrackerWebApi.HttpApi.Auth;
using System.Security.Claims;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.HttpApi.Extensions;

public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    private ClaimsPrincipal? user;

    public string? Name => this.user?.Identity?.Name;

    private Guid userId = Guid.Empty;

    public Guid GetUserId() =>
        this.IsAuthenticated()
            ? this.user?.GetUserId() ?? Guid.Empty
            : this.userId;

    public string? GetUserEmail() =>
        this.IsAuthenticated()
            ? this.user!.GetEmail()
            : string.Empty;

    public bool IsAuthenticated() =>
        this.user?.Identity?.IsAuthenticated is true;

    public bool IsInRole(string role) =>
        this.user?.IsInRole(role) is true;

    public IEnumerable<Claim>? GetUserClaims() =>
        this.user?.Claims;

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (this.user != null)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        this.user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        if (this.userId != Guid.Empty)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        if (!string.IsNullOrEmpty(userId))
        {
            this.userId = Guid.Parse(userId);
        }
    }
}
