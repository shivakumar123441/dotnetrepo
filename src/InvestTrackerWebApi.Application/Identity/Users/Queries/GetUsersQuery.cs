namespace InvestTrackerWebApi.Application.Identity.Users;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoMapper;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class GetUsersQuery : OrderableFilterBase, IRequest<List<UserDto>>
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

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IMapper mapper;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        this.userManager = userManager;
        this.mapper = mapper;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await this.userManager.Users.ApplyFilter(request).ToListAsync();

        return this.mapper.Map<List<UserDto>>(users);
    }
}
