namespace InvestTrackerWebApi.Application.Identity.Users;

using System.Threading.Tasks;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class GetCurrentUserPermissionsQuery : IRequest<List<string>>
{
}

public class GetUserPermissionsQueryHandler : IRequestHandler<GetCurrentUserPermissionsQuery, List<string>>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IIdentityDbContext identityDbContext;
    private readonly ICurrentUser currentUser;

    public GetUserPermissionsQueryHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IIdentityDbContext identityDbContext, ICurrentUser currentUser)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.identityDbContext = identityDbContext;
        this.currentUser = currentUser;
    }

    public async Task<List<string>> Handle(GetCurrentUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.FindByIdAsync(this.currentUser.GetUserId().ToString());

        _ = user ?? throw new NotFoundException("User Not Found.");

        var userRoles = await this.userManager.GetRolesAsync(user);
        var permissions = new List<string>();
        foreach (var role in await this.roleManager.Roles
            .Where(r => userRoles.Contains(r.Name))
            .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await this.identityDbContext.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == ClaimTypes.Permission)
                .Select(rc => rc.ClaimValue)
                .ToListAsync(cancellationToken));
        }

        return permissions.Distinct().ToList();
    }
}
