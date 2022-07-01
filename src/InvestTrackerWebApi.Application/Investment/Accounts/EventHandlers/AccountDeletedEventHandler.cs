namespace InvestTrackerWebApi.Application.Accounts.EventHandlers;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Domain.Account.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class AccountDeletedEventHandler : INotificationHandler<EventNotification<AccountDeletedEvent>>
{
    private readonly ILogger<AccountDeletedEventHandler> logger;

    public AccountDeletedEventHandler(ILogger<AccountDeletedEventHandler> logger) => this.logger = logger;

    public Task Handle(EventNotification<AccountDeletedEvent> notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
