namespace InvestTrackerWebApi.Application.Accounts.EventHandlers;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Domain.Account.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class AccountCreatedEventHandler : INotificationHandler<EventNotification<AccountCreatedEvent>>
{
    private readonly ILogger<AccountCreatedEventHandler> logger;

    public AccountCreatedEventHandler(ILogger<AccountCreatedEventHandler> logger) => this.logger = logger;

    public Task Handle(EventNotification<AccountCreatedEvent> notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
