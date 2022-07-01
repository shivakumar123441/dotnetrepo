namespace InvestTrackerWebApi.Application.Transactions;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteTransactionCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteTransactionCommand(Guid id) => this.Id = id;
}

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Guid>
{
    private readonly IApplicationDbContext applicationDbContext;

    public DeleteTransactionCommandHandler(IApplicationDbContext applicationDbContext) =>
        this.applicationDbContext = applicationDbContext;

    public async Task<Guid> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {

        var entity = await this.applicationDbContext.Transactions.Where(l => l.Id == request.Id)
                    .SingleOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(string.Format("{0} account not found", request.Id));
        }

        _ = this.applicationDbContext.Transactions.Remove(entity);

        _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}
