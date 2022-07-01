namespace InvestTrackerWebApi.Application.Transactions;

using AutoMapper;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetCurrentUserTransactionQuery : IRequest<CurrentUserTransactionDetailsDto>
{
    public Guid Id { get; set; } = default!;

    public Guid UserId { get; set; }
}

public class GetUserTransactionQueryHandler : IRequestHandler<GetCurrentUserTransactionQuery, CurrentUserTransactionDetailsDto>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;

    public GetUserTransactionQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
    }

    public async Task<CurrentUserTransactionDetailsDto> Handle(GetCurrentUserTransactionQuery request, CancellationToken cancellationToken)
    {
        var accountIds = this.applicationDbContext.Accounts.Where(account => account.UserId == request.UserId).Select(account => account.Id).ToList();
        var transaction = await this.applicationDbContext.Transactions
            .Where(transaction => transaction.Id == request.Id && accountIds.Contains(transaction.AccountId))
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(cancellationToken);

        if (transaction == null)
        {
            throw new NotFoundException(request.Id + " transaction not found for user " + request.UserId);
        }

        return this.mapper.Map<CurrentUserTransactionDetailsDto>(transaction);
    }
}
