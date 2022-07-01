namespace InvestTrackerWebApi.Application.Accounts;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoMapper;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetAccountsQuery : OrderableFilterBase, IRequest<List<AccountDto>>
{
    [ToLowerContainsComparison]
    public string? Name { get; set; }

    [ToLowerContainsComparison]
    public string? Description { get; set; }
}

public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<AccountDto>>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;

    public GetAccountsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
    }

    public async Task<List<AccountDto>> Handle(
        GetAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await this.applicationDbContext.Accounts.ApplyFilter(request).ToListAsync(cancellationToken);

        return this.mapper.Map<List<AccountDto>>(accounts);
    }
}
