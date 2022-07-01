namespace InvestTrackerWebApi.Application.Identity.Users;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class ToggleUserStatusCommand : IRequest
{
    public Guid UserId { get; set; }
    public bool ActivateUser { get; set; }
}

public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ICurrentUser currentUser;

    public ToggleUserStatusCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
    {
        this.userManager = userManager;
        this.currentUser = currentUser;
    }

    public async Task<Unit> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.Users.Where(u => u.Id == request.UserId.ToString())
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException("User Not Found.");

        if (user.Email.ToLowerInvariant() == RootConstants.RootAdminUserEmail.ToLowerInvariant())
        {
            throw new IdentityException("Operation not allowed.");
        }

        user = user.ToggleUserStatus(request.ActivateUser, this.currentUser.GetUserId());
        user.AddDomainEvent(new ApplicationUserUpdatedEvent(user));
        await this.userManager.UpdateAsync(user);
        return Unit.Value;
    }
}
