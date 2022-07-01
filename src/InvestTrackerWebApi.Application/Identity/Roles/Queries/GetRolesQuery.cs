namespace InvestTrackerWebApi.Application.Identity.Roles;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoMapper;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class GetRolesQuery : OrderableFilterBase, IRequest<List<RoleDto>>
{
    [ToLowerContainsComparison]
    public string? Name { get; set; }

    [ToLowerContainsComparison]
    public string? Description { get; set; }
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IMapper mapper;

    public GetRolesQueryHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    {
        this.roleManager = roleManager;
        this.mapper = mapper;
    }

    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await this.roleManager.Roles.ApplyFilter(request).ToListAsync();

        return this.mapper.Map<List<RoleDto>>(roles);
    }
}
