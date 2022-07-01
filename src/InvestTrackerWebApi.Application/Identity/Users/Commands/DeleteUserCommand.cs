namespace InvestTrackerWebApi.Application.Identity.Users;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class DeleteUserCommand : IRequest<string>
{
    public Guid Id { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, string>
{
    private readonly UserManager<ApplicationUser> userManager;

    public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager) =>
        this.userManager = userManager;

    public async Task<string> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.FindByIdAsync(request.Id.ToString());

        _ = user ?? throw new NotFoundException("User Not Found");

        if (user.Email.ToLowerInvariant() == RootConstants.RootAdminUserEmail.ToLowerInvariant())
        {
            throw new IdentityException("Operation not allowed.");
        }

        user.AddDomainEvent(new ApplicationUserDeletedEvent(user));

        await this.userManager.DeleteAsync(user);

        return string.Format("User {0} Deleted.", user.UserName);
    }
}
