namespace InvestTrackerWebApi.Application.Identity.Users;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class ResetPasswordCommand : IRequest<string>
{
    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Token { get; set; }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, string>
{
    private readonly UserManager<ApplicationUser> userManager;

    public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager) =>
        this.userManager = userManager;

    public async Task<string> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.FindByEmailAsync(request.Email?.Normalize());

        // Don't reveal that the user does not exist
        _ = user ?? throw new IdentityException("An Error has occurred!");

        var result = await this.userManager.ResetPasswordAsync(user, request.Token, request.Password);

        if (!result.Succeeded)
        {
            throw new ValidationException(
                "Validation Errors Occurred.",
                result.Errors.GroupBy(e => e.Code, e => e.Description)
                    .ToDictionary(
                    failureGroup =>
                    failureGroup.Key,
                    failureGroup => failureGroup.ToArray()));
        }

        return "Password Reset Successful!";
    }
}
