namespace InvestTrackerWebApi.Application.Accounts;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;

public class GetAccountsWithPaginationQuery : PaginationQueryBase, IRequest<PaginatedList<AccountDto>>
{
    [ToLowerContainsComparison]
    public string? Name { get; set; }

    [ToLowerContainsComparison]
    public string? Description { get; set; }
}

public class GetAccountsWithPaginationQueryHandler : IRequestHandler<GetAccountsWithPaginationQuery, PaginatedList<AccountDto>>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;

    public GetAccountsWithPaginationQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
    }

    public async Task<PaginatedList<AccountDto>> Handle(GetAccountsWithPaginationQuery request, CancellationToken cancellationToken) =>
        await this.applicationDbContext.Accounts.ApplyFilter(request)
        .ProjectTo<AccountDto>(this.mapper.ConfigurationProvider)
        .PaginatedListAsync(request.CurrentPage, request.PageSize);
}
