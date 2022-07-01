namespace InvestTrackerWebApi.Application.Identity.EventHandlers;

using InvestTrackerWebApi.Application.Accounts;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using InvestTrackerWebApi.Domain.Identity.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public class ApplicationUserCreatedEventHandler : INotificationHandler<EventNotification<ApplicationUserCreatedEvent>>
{
    private readonly ILogger<ApplicationUserCreatedEventHandler> logger;
    private readonly ISender mediator;
    private readonly UserManager<ApplicationUser> userManager;

    public ApplicationUserCreatedEventHandler(ILogger<ApplicationUserCreatedEventHandler> logger, ISender mediator, UserManager<ApplicationUser> userManager)
    {
        this.logger = logger;
        this.mediator = mediator;
        this.userManager = userManager;
    }

    public async Task Handle(EventNotification<ApplicationUserCreatedEvent> notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);

        await this.mediator.Send(new CreateWalletAccountCommand() { UserId = Guid.Parse(notification.DomainEvent.ApplicationUser.Id) });

        if (!string.IsNullOrEmpty(notification.DomainEvent.ApplicationUser.ReferralEmail))
        {
            var user = this.userManager.Users.Where(x => x.Email == notification.DomainEvent.ApplicationUser.ReferralEmail).First();
            await this.mediator.Send(new CreateReferralAccountCommand() { UserId = Guid.Parse(user.Id) });
        }
    }
}
