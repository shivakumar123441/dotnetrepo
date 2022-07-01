namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole.Events;
using InvestTrackerWebApi.Domain.Common;

public class ApplicationRoleDeletedEvent : DomainEvent
{
    public ApplicationRoleDeletedEvent(ApplicationRole applicationRole) => this.ApplicationRole = applicationRole;

    public ApplicationRole ApplicationRole { get; }
}
