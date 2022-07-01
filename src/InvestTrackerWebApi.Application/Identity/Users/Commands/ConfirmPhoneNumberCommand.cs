namespace InvestTrackerWebApi.Application.Identity.Users;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class ConfirmPhoneNumberCommand : IRequest<string>
{
    public Guid UserId { get; set; } = default!;
    public string Code { get; set; } = default!;
}

public class ConfirmPhoneNumberCommandHandler : IRequestHandler<ConfirmPhoneNumberCommand, string>
{
    private readonly UserManager<ApplicationUser> userManager;

    public ConfirmPhoneNumberCommandHandler(UserManager<ApplicationUser> userManager) =>
        this.userManager = userManager;

    public async Task<string> Handle(ConfirmPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.FindByIdAsync(request.UserId.ToString());

        _ = user ?? throw new IdentityException("An error occurred while confirming Mobile Phone.");

        var result = await this.userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, request.Code);

        return result.Succeeded
            ? user.EmailConfirmed
                ? string.Format(
                    "Account Confirmed for Phone Number {0}." +
                    " You can now use the /api/tokens endpoint to generate JWT.",
                    user.PhoneNumber)
                : string.Format(
                    "Account Confirmed for Phone Number {0}." +
                    " You should confirm your E-mail before using the /api/tokens endpoint to generate JWT.",
                    user.PhoneNumber)
            : throw new IdentityException(string.Format(
                "An error occurred while confirming {0}", user.PhoneNumber));
    }
}
