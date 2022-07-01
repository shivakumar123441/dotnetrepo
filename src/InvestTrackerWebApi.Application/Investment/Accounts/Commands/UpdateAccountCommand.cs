namespace InvestTrackerWebApi.Application.Accounts;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;

public class UpdateAccountCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;
}

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Guid>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly ICurrentUser currentUser;

    public UpdateAccountCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUser currentUser)
    {
        this.applicationDbContext = applicationDbContext;
        this.currentUser = currentUser;
    }

    public async Task<Guid> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await this.applicationDbContext.Accounts.FindAsync(request.Id, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException(string.Format("{0} account not found", request.Id));
        }

        account.Update(request.Description, this.currentUser.GetUserId());

        _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}
