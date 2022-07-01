namespace InvestTrackerWebApi.Application.Identity.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Extensions;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Domain.Configurations;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class GetRefreshTokenQuery : IRequest<TokenResponse>
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}

public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, TokenResponse>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly JwtSettings jwtSettings;
    private readonly ICurrentUser currentUser;

    public GetRefreshTokenQueryHandler(
        UserManager<ApplicationUser> userManager,
        ICurrentUser currentUser,
        IOptions<JwtSettings> jwtSettings)
    {
        this.userManager = userManager;
        this.currentUser = currentUser;
        this.jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenResponse> Handle(GetRefreshTokenQuery refreshTokenQuery, CancellationToken cancellationToken)
    {
        var userPrincipal = this.GetPrincipalFromExpiredToken(refreshTokenQuery.Token);
        string? userEmail = userPrincipal.GetEmail();
        var user = await this.userManager.FindByEmailAsync(userEmail);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (user.RefreshToken != refreshTokenQuery.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        return await this.GenerateTokensAndUpdateUser(user);
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user)
    {
        string token = this.GenerateJwt(user);
        user = user.UpdateRefreshTokenParameters(
            this.GenerateRefreshToken(),
            DateTime.UtcNow.AddDays(this.jwtSettings.RefreshTokenExpirationInDays),
            this.currentUser.GetUserId());
        await this.userManager.UpdateAsync(user);

        return new TokenResponse(token, user.RefreshToken!, user.RefreshTokenExpiryTime);
    }

    private string GenerateJwt(ApplicationUser user) =>
        this.GenerateEncryptedToken(this.GetSigningCredentials(), this.GetClaims(user));

    private IEnumerable<Claim> GetClaims(ApplicationUser user) =>
        new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(Domain.Identity.ClaimTypes.Fullname, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(Domain.Identity.ClaimTypes.ImageUrl, user.ImageUrl),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };

    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(this.jwtSettings.TokenExpirationInMinutes),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        if (string.IsNullOrEmpty(this.jwtSettings.Key))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedAccessException("Invalid token.");
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        if (string.IsNullOrEmpty(this.jwtSettings.Key))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        byte[] secret = Encoding.UTF8.GetBytes(this.jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}


