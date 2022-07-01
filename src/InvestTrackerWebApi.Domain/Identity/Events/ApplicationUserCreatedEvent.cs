namespace InvestTrackerWebApi.Domain.Identity.Events;
using InvestTrackerWebApi.Domain.Common;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;

public class ApplicationUserCreatedEvent : DomainEvent
{
    public ApplicationUserCreatedEvent(ApplicationUser applicationUser) => this.ApplicationUser = applicationUser;

    public ApplicationUser ApplicationUser { get; }
}
