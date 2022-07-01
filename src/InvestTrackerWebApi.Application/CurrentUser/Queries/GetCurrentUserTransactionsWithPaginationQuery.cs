namespace InvestTrackerWebApi.Application.Transactions;

using System.Linq;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoMapper;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Application.Persistence;
using InvestTrackerWebApi.Domain.Transaction;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetCurrentUserTransactionsWithPaginationQuery : PaginationQueryBase, IRequest<PaginatedList<CurrentUserTransactionDto>>
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

public class GetUserTransactionsWithPaginationQueryHandler : IRequestHandler<GetCurrentUserTransactionsWithPaginationQuery, PaginatedList<CurrentUserTransactionDto>>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;
    private readonly ICurrentUser currentUser;

    public GetUserTransactionsWithPaginationQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICurrentUser currentUser)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
        this.currentUser = currentUser;
    }

    public async Task<PaginatedList<CurrentUserTransactionDto>> Handle(
        GetCurrentUserTransactionsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var accountIds = this.applicationDbContext.Accounts.Where(account => account.UserId == this.currentUser.GetUserId()).Select(account => account.Id).ToList();
        var paginatedtransactionsList = await this.applicationDbContext.Transactions
            .Where(transaction => accountIds.Contains(transaction.AccountId))
            .Include(t => t.Attachments)
            .ApplyFilter(request)
            .PaginatedListAsync(request.CurrentPage, request.PageSize);
        return new PaginatedList<CurrentUserTransactionDto>(
            this.mapper.Map<List<CurrentUserTransactionDto>>(paginatedtransactionsList.Items),
            paginatedtransactionsList.TotalCount,
            paginatedtransactionsList.PageNumber,
            request.PageSize);
    }
}
