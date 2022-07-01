namespace InvestTrackerWebApi.Application.Transactions;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoMapper;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Application.Persistence;
using InvestTrackerWebApi.Domain.Transaction;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetTransactionsWithPaginationQuery : PaginationQueryBase, IRequest<PaginatedList<TransactionDto>>
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

public class GetTransactionsWithPaginationQueryHandler : IRequestHandler<GetTransactionsWithPaginationQuery, PaginatedList<TransactionDto>>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;

    public GetTransactionsWithPaginationQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
    }

    public async Task<PaginatedList<TransactionDto>> Handle(GetTransactionsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paginatedtransactionsList = await this.applicationDbContext.Transactions.ApplyFilter(request)
            .Include(t => t.Attachments)
            .PaginatedListAsync(request.CurrentPage, request.PageSize);
        return new PaginatedList<TransactionDto>(
            this.mapper.Map<List<TransactionDto>>(paginatedtransactionsList.Items),
            paginatedtransactionsList.TotalCount,
            paginatedtransactionsList.PageNumber,
            request.PageSize);
    }
}
