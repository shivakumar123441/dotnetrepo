namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser.Events;
using InvestTrackerWebApi.Domain.Common;

public class ApplicationUserUpdatedEvent : DomainEvent
{
    public ApplicationUserUpdatedEvent(ApplicationUser applicationUser) =>
        this.ApplicationUser = applicationUser;

    public ApplicationUser ApplicationUser { get; }
}
