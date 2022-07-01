namespace InvestTrackerWebApi.HttpApi.Extensions;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.Email);

    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("UserId is empty.");
        }

        return Guid.Parse(userId);
    }

    private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
        principal is null
            ? throw new ArgumentNullException(nameof(principal))
            : principal.FindFirst(claimType)?.Value;
}
