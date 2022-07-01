namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRoleClaim.Events;
using InvestTrackerWebApi.Domain.Common;

public class ApplicationRoleClaimUpdatedEvent : DomainEvent
{
    public ApplicationRoleClaimUpdatedEvent(ApplicationRoleClaim applicationRoleClaim) =>
        this.ApplicationRoleClaim = applicationRoleClaim;

    public ApplicationRoleClaim ApplicationRoleClaim { get; }
}
