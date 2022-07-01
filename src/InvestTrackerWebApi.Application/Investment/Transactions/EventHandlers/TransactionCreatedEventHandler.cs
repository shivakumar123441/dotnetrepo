namespace InvestTrackerWebApi.Application.Transactions.EventHandlers;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Domain.Transaction.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class TransactionCreatedEventHandler : INotificationHandler<EventNotification<TransactionCreatedEvent>>
{
    private readonly ILogger<TransactionCreatedEventHandler> logger;

    public TransactionCreatedEventHandler(ILogger<TransactionCreatedEventHandler> logger) => this.logger = logger;

    public Task Handle(EventNotification<TransactionCreatedEvent> notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
