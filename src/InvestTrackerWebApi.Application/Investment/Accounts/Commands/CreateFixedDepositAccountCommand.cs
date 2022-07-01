namespace InvestTrackerWebApi.Application.Accounts;

using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;

public class CreateFixedDepositAccountCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
}

public class CreateFixedDepositAccountCommandHandler : IRequestHandler<CreateFixedDepositAccountCommand, Guid>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly ICurrentUser currentUser;

    public CreateFixedDepositAccountCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUser currentUser)
    {
        this.applicationDbContext = applicationDbContext;
        this.currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateFixedDepositAccountCommand request, CancellationToken cancellationToken)
    {
        var currentCount = this.applicationDbContext.Accounts.Where(x => x.AccountType == Domain.Account.AccountType.FixedDeposit).Count();
        var referenceCode = "FD" + string.Format("{0:00000000}", currentCount + 1);
        var account = new Domain.Account.Account(Guid.NewGuid(), referenceCode, "Fixed Deposit Account", Domain.Account.AccountType.FixedDeposit, Domain.Account.AccountStatus.Created, request.UserId, this.currentUser.GetUserId());

        _ = await this.applicationDbContext.Accounts.AddAsync(account, cancellationToken);

        _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}
