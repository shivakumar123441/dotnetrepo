namespace InvestTrackerWebApi.Application.Identity.Roles;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole.Events;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class DeleteRoleCommand : IRequest<string>
{
    public Guid Id { get; set; }
}

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, string>
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;

    public DeleteRoleCommandHandler(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
    }

    public async Task<string> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await this.roleManager.FindByIdAsync(request.Id.ToString());

        _ = role ?? throw new NotFoundException("Role Not Found");

        if (role.Name.ToLowerInvariant() == RootConstants.RootAdminRole.ToLowerInvariant())
        {
            throw new IdentityException("Operation not allowed.");
        }

        if ((await this.userManager.GetUsersInRoleAsync(role.Name)).Any())
        {
            throw new IdentityException(
                string.Format("Not allowed to delete {0} Role as it is being used.", role.Name));
        }

        role.AddDomainEvent(new ApplicationRoleDeletedEvent(role));
        await this.roleManager.DeleteAsync(role);

        return string.Format("Role {0} Deleted.", role.Name);
    }
}
