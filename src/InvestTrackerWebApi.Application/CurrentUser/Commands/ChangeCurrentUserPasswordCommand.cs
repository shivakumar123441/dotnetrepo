namespace InvestTrackerWebApi.Application.Identity.Users;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class ChangeCurrentUserPasswordCommand : IRequest
{
    public string Password { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmNewPassword { get; set; } = default!;
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangeCurrentUserPasswordCommand>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ICurrentUser currentUser;

    public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
    {
        this.userManager = userManager;
        this.currentUser = currentUser;
    }

    public async Task<Unit> Handle(ChangeCurrentUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await this.userManager.FindByIdAsync(this.currentUser.GetUserId().ToString());

        _ = user ?? throw new NotFoundException("User Not Found.");

        var result = await this.userManager.ChangePasswordAsync(user, command.Password, command.NewPassword);

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

        return Unit.Value;
    }
}
