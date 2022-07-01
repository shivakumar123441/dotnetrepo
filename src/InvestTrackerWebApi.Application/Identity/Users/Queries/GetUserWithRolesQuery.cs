namespace InvestTrackerWebApi.Application.Identity.Users;

using AutoMapper;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class GetUserWithRolesQuery : IRequest<UserWithRolesDto>
{
    public Guid Id { get; set; } = default!;
}

public class GetUserRolesQueryHandler : IRequestHandler<GetUserWithRolesQuery, UserWithRolesDto>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;

    public GetUserRolesQueryHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public async Task<UserWithRolesDto> Handle(GetUserWithRolesQuery request, CancellationToken cancellationToken)
    {
        var userWithRoles = new UserWithRolesDto();
        var userRoles = new List<UserRoleDto>();

        var user = await this.userManager.FindByIdAsync(request.Id.ToString());
        var roles = await this.roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);
        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleDto
            {
                RoleId = Guid.Parse(role.Id),
                RoleName = role.Name,
                Description = role.Description,
                Enabled = await this.userManager.IsInRoleAsync(user, role.Name)
            });
        }

        userWithRoles.Id = Guid.Parse(user.Id);
        userWithRoles.UserRolesDto = userRoles;
        return userWithRoles;
    }
}
