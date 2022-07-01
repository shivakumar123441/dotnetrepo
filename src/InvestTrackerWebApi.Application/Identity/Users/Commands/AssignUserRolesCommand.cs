namespace InvestTrackerWebApi.Application.Identity.Users;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class AssignUserRolesCommand : IRequest<string>
{
    public Guid UserId { get; set; } = default!;

    public List<UserRoleDto> UserRoles { get; set; } = new();
}

public class AssignUserRolesCommandHandler : IRequestHandler<AssignUserRolesCommand, string>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;

    public AssignUserRolesCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public async Task<string> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.Users.Where(u => u.Id == request.UserId.ToString()).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException("User Not Found.");

        if (user.Email.ToLowerInvariant() == RootConstants.RootAdminUserEmail.ToLowerInvariant())
        {
            throw new IdentityException("Operation not allowed.");
        }

        user.AddDomainEvent(new ApplicationUserUpdatedEvent(user));

        foreach (var userRole in request.UserRoles)
        {
            // Check if Role Exists
            if (await this.roleManager.FindByNameAsync(userRole.RoleName) is not null)
            {
                if (userRole.Enabled)
                {
                    if (!await this.userManager.IsInRoleAsync(user, userRole.RoleName))
                    {
                        await this.userManager.AddToRoleAsync(user, userRole.RoleName);
                    }
                }
                else
                {
                    await this.userManager.RemoveFromRoleAsync(user, userRole.RoleName);
                }
            }
        }

        return "User Roles Updated Successfully.";
    }
}
