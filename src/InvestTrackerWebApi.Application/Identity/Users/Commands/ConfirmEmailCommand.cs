namespace InvestTrackerWebApi.Application.Identity.Users;

using System.Text;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

public class ConfirmEmailCommand : IRequest<string>
{
    public Guid UserId { get; set; } = default!;
    public string Code { get; set; } = default!;
}

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, string>
{
    private readonly UserManager<ApplicationUser> userManager;

    public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager) =>
        this.userManager = userManager;

    public async Task<string> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.Users.IgnoreQueryFilters()
            .Where(u => u.Id == request.UserId.ToString() && !u.EmailConfirmed)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new IdentityException("An error occurred while confirming E-Mail.");

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        var result = await this.userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded
            ? string.Format(
                "Email Id '{0}' is successfully verified. " +
                "Please contact the administrators to for furture actions.",
                user.Email)
            : throw new IdentityException(
                string.Format(
                    "An error occurred while confirming {0}",
                    user.Email));
    }
}
