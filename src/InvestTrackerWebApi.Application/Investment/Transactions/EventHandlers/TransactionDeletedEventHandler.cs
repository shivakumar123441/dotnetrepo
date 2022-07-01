namespace InvestTrackerWebApi.Application.Transactions.EventHandlers;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Domain.Transaction.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class TransactionDeletedEventHandler : INotificationHandler<EventNotification<TransactionDeletedEvent>>
{
    private readonly ILogger<TransactionDeletedEventHandler> logger;

    public TransactionDeletedEventHandler(ILogger<TransactionDeletedEventHandler> logger) => this.logger = logger;

    public Task Handle(EventNotification<TransactionDeletedEvent> notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
