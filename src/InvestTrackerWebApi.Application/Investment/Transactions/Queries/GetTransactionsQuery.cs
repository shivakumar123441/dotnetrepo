namespace InvestTrackerWebApi.Application.Transactions;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoMapper;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InvestTrackerWebApi.Domain.Transaction;

public class GetTransactionsQuery : OrderableFilterBase, IRequest<List<TransactionDto>>
{
    public Guid? AccountId { get; set; }
    [ToLowerContainsComparison]
    public string? ReferenceCode { get; set; }
    [ToLowerContainsComparison]
    public string? UserComments { get; set; }
    [ToLowerContainsComparison]
    public string? TransactionComments { get; set; }
    public Domain.Transaction.TransactionType? TransactionType { get; set; }
    public DateTime? MadeOn { get; set; }
    public decimal? Amount { get; set; }
    public TransactionStatus? TransactionStatus { get; set; }
}

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, List<TransactionDto>>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;

    public GetTransactionsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
    }

    public async Task<List<TransactionDto>> Handle(
        GetTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await this.applicationDbContext.Transactions
            .Include(t => t.Attachments)
            .ApplyFilter(request)
            .ToListAsync(cancellationToken);

        return this.mapper.Map<List<TransactionDto>>(transactions);
    }
}
