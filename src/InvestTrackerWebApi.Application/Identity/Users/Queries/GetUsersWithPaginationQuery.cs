namespace InvestTrackerWebApi.Application.Identity.Users;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class GetUsersWithPaginationQuery : PaginationQueryBase, IRequest<PaginatedList<UserDto>>
{
    [ToLowerContainsComparison]
    public string? UserName { get; set; }

    [ToLowerContainsComparison]
    public string? FirstName { get; set; }

    [ToLowerContainsComparison]
    public string? LastName { get; set; }

    [ToLowerContainsComparison]
    public string? Email { get; set; }

    [ToLowerContainsComparison]
    public string? PhoneNumber { get; set; }
}

public class GetUsersWithPaginationQueryHandler : IRequestHandler<GetUsersWithPaginationQuery, PaginatedList<UserDto>>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IMapper mapper;

    public GetUsersWithPaginationQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        this.userManager = userManager;
        this.mapper = mapper;
    }

    public async Task<PaginatedList<UserDto>> Handle(GetUsersWithPaginationQuery request, CancellationToken cancellationToken) => await this.userManager.Users
       .ApplyFilter(request)
       .ProjectTo<UserDto>(this.mapper.ConfigurationProvider)
       .PaginatedListAsync(request.CurrentPage, request.PageSize);
}
