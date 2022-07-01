namespace InvestTrackerWebApi.Application.Identity.Roles;

using AutoMapper;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class GetRoleWithPermissionsQuery : IRequest<RoleWithPermissionsDto>
{
    public Guid Id { get; set; }
}

public class GetRoleWithPermissionsQueryHandler : IRequestHandler<GetRoleWithPermissionsQuery, RoleWithPermissionsDto>
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IMapper mapper;
    private readonly IIdentityDbContext identityDbContext;

    public GetRoleWithPermissionsQueryHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper, IIdentityDbContext identityDbContext)
    {
        this.roleManager = roleManager;
        this.mapper = mapper;
        this.identityDbContext = identityDbContext;
    }

    public async Task<RoleWithPermissionsDto> Handle(GetRoleWithPermissionsQuery request, CancellationToken cancellationToken)
    {
        var roleEntity = await this.roleManager.Roles.SingleOrDefaultAsync(x => x.Id == request.Id.ToString(), cancellationToken);

        if (roleEntity == null)
        {
            throw new NotFoundException("Role Not Found");
        }

        var role = this.mapper.Map<RoleDetailsDto>(roleEntity);
        var roleWithPermissionsDto = this.mapper.Map<RoleWithPermissionsDto>(role);
        roleWithPermissionsDto.Permissions = await this.identityDbContext.RoleClaims
                .Where(a => a.RoleId == request.Id.ToString() && a.ClaimType == Domain.Identity.ClaimTypes.Permission)
                .Select(c => c.ClaimValue)
                .ToListAsync(cancellationToken);

        return roleWithPermissionsDto;
    }
}
