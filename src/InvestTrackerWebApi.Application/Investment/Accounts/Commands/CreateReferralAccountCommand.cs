namespace InvestTrackerWebApi.Application.Accounts;

using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;

public class CreateReferralAccountCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
}

public class CreateReferralAccountCommandHandler : IRequestHandler<CreateReferralAccountCommand, Guid>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly ICurrentUser currentUser;

    public CreateReferralAccountCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUser currentUser)
    {
        this.applicationDbContext = applicationDbContext;
        this.currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateReferralAccountCommand request, CancellationToken cancellationToken)
    {
        var currentCount = this.applicationDbContext.Accounts.Where(x => x.AccountType == Domain.Account.AccountType.Referral).Count();
        var accountName = "RE" + string.Format("{0:00000000}", currentCount + 1);
        var account = new Domain.Account.Account(Guid.NewGuid(), accountName, "Referral Account", Domain.Account.AccountType.Referral, Domain.Account.AccountStatus.Created, request.UserId, this.currentUser.GetUserId());

        _ = await this.applicationDbContext.Accounts.AddAsync(account, cancellationToken);

        _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}
