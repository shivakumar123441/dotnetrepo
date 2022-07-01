namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser.Events;
using InvestTrackerWebApi.Domain.Common;

public class ApplicationUserCreatedEvent : DomainEvent
{
    public ApplicationUserCreatedEvent(ApplicationUser applicationUser) =>
        this.ApplicationUser = applicationUser;

    public ApplicationUser ApplicationUser { get; }
}
