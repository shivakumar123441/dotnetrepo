namespace InvestTrackerWebApi.Application.Identity.Roles;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class GetRolesWithPaginationQuery : PaginationQueryBase, IRequest<PaginatedList<RoleDto>>
{
    [ToLowerContainsComparison]
    public string? Name { get; set; }

    [ToLowerContainsComparison]
    public string? Description { get; set; }
}

public class GetRolesWithPaginationQueryHandler : IRequestHandler<GetRolesWithPaginationQuery, PaginatedList<RoleDto>>
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IMapper mapper;

    public GetRolesWithPaginationQueryHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    {
        this.roleManager = roleManager;
        this.mapper = mapper;
    }

    public async Task<PaginatedList<RoleDto>> Handle(GetRolesWithPaginationQuery request, CancellationToken cancellationToken) => await this.roleManager.Roles
       .ApplyFilter(request)
       .ProjectTo<RoleDto>(this.mapper.ConfigurationProvider)
       .PaginatedListAsync(request.CurrentPage, request.PageSize);
}
