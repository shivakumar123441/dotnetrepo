namespace InvestTrackerWebApi.Application.Transactions.EventHandlers;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Domain.Transaction.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class TransactionUpdatedEventHandler : INotificationHandler<EventNotification<TransactionUpdatedEvent>>
{
    private readonly ILogger<TransactionUpdatedEventHandler> logger;

    public TransactionUpdatedEventHandler(ILogger<TransactionUpdatedEventHandler> logger) => this.logger = logger;

    public Task Handle(EventNotification<TransactionUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
