namespace InvestTrackerWebApi.Application.Identity.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Domain.Configurations;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class GetTokenQuery : IRequest<TokenResponse>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class GetTokenQueryHandler : IRequestHandler<GetTokenQuery, TokenResponse>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly JwtSettings jwtSettings;
    private readonly ICurrentUser currentUser;

    public GetTokenQueryHandler(
        UserManager<ApplicationUser> userManager,
        ICurrentUser currentUser,
        IOptions<JwtSettings> jwtSettings)
    {
        this.userManager = userManager;
        this.currentUser = currentUser;
        this.jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenResponse> Handle(GetTokenQuery tokenQuery, CancellationToken cancellationToken)
    {
        var user = await this.userManager.FindByEmailAsync(tokenQuery.Email.Trim().Normalize());
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedAccessException("User email id not Verified");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User is not active.");
        }


        if (!await this.userManager.CheckPasswordAsync(user, tokenQuery.Password))
        {
            throw new UnauthorizedAccessException("Password is incorrect.");
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
