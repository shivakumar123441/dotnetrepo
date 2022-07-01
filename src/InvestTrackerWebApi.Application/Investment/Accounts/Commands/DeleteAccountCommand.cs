namespace InvestTrackerWebApi.Application.Accounts;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteAccountCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteAccountCommand(Guid id) => this.Id = id;
}

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Guid>
{
    private readonly IApplicationDbContext applicationDbContext;

    public DeleteAccountCommandHandler(IApplicationDbContext applicationDbContext) =>
        this.applicationDbContext = applicationDbContext;

    public async Task<Guid> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {

        var entity = await this.applicationDbContext.Accounts.Where(l => l.Id == request.Id)
                    .SingleOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(string.Format("{0} account not found", request.Id));
        }

        _ = this.applicationDbContext.Accounts.Remove(entity);

        _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}
