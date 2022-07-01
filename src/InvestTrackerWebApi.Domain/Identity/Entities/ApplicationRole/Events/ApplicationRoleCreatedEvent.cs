namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole.Events;
using InvestTrackerWebApi.Domain.Common;

public class ApplicationRoleCreatedEvent : DomainEvent
{
    public ApplicationRoleCreatedEvent(ApplicationRole applicationRole) => this.ApplicationRole = applicationRole;

    public ApplicationRole ApplicationRole { get; }
}
