namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser.Events;
using InvestTrackerWebApi.Domain.Common;

public class ApplicationUserDeletedEvent : DomainEvent
{
    public ApplicationUserDeletedEvent(ApplicationUser applicationUser) =>
        this.ApplicationUser = applicationUser;

    public ApplicationUser ApplicationUser { get; }
}
