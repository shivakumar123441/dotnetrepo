namespace InvestTrackerWebApi.Application.Accounts.EventHandlers;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Domain.Account.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class AccountUpdatedEventHandler : INotificationHandler<EventNotification<AccountUpdatedEvent>>
{
    private readonly ILogger<AccountUpdatedEventHandler> logger;

    public AccountUpdatedEventHandler(ILogger<AccountUpdatedEventHandler> logger) => this.logger = logger;

    public Task Handle(EventNotification<AccountUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
