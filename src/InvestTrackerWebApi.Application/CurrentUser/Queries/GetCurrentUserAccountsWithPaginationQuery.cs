namespace InvestTrackerWebApi.Application.Accounts;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;

public class GetCurrentUserAccountsWithPaginationQuery : PaginationQueryBase, IRequest<PaginatedList<CurrentUserAccountDto>>
{
    [ToLowerContainsComparison]
    public string? Name { get; set; }

    [ToLowerContainsComparison]
    public string? Description { get; set; }
}

public class GetUserAccountsWithPaginationQueryHandler : IRequestHandler<GetCurrentUserAccountsWithPaginationQuery, PaginatedList<CurrentUserAccountDto>>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;
    private readonly ICurrentUser currentUser;

    public GetUserAccountsWithPaginationQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICurrentUser currentUser)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
        this.currentUser = currentUser;
    }

    public async Task<PaginatedList<CurrentUserAccountDto>> Handle(
        GetCurrentUserAccountsWithPaginationQuery request,
        CancellationToken cancellationToken) => await this.applicationDbContext.Accounts.Where(account => account.UserId == this.currentUser.GetUserId()).ApplyFilter(request)
        .ProjectTo<CurrentUserAccountDto>(this.mapper.ConfigurationProvider)
        .PaginatedListAsync(request.CurrentPage, request.PageSize);
}
