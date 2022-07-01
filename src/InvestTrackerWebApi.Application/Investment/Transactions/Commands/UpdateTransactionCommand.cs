namespace InvestTrackerWebApi.Application.Transactions;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;
using Domain.Transaction;

public class UpdateTransactionCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public string? UserComments { get; set; } = default!;

    public string? TransactionComments { get; set; } = default!;
}

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, Guid>
{
    private readonly IApplicationDbContext applicationDbContext;

    public UpdateTransactionCommandHandler(IApplicationDbContext applicationDbContext) =>
        this.applicationDbContext = applicationDbContext;

    public async Task<Guid> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await this.applicationDbContext.Transactions.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (transaction is null)
        {
            throw new NotFoundException(string.Format("{0} transaction not found", request.Id));
        }

        if (transaction.TransactionStatus is not TransactionStatus.Created)
        {
            throw new InvalidOperationException("Cannot update freezed transaction");
        }

        if (request.UserComments is not null)
        {
            _ = transaction.UpdateUserComments(request.UserComments);
        }

        if (request.TransactionComments is not null)
        {
            _ = transaction.UpdateTransactionComments(request.TransactionComments);
        }

        _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}
