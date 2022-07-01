namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole.Events;
using InvestTrackerWebApi.Domain.Common;

public class ApplicationRoleUpdatedEvent : DomainEvent
{
    public ApplicationRoleUpdatedEvent(ApplicationRole applicationRole) => this.ApplicationRole = applicationRole;

    public ApplicationRole ApplicationRole { get; }
}
