namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRoleClaim.Events;
using InvestTrackerWebApi.Domain.Common;

public class ApplicationRoleClaimDeletedEvent : DomainEvent
{
    public ApplicationRoleClaimDeletedEvent(ApplicationRoleClaim applicationRoleClaim) =>
        this.ApplicationRoleClaim = applicationRoleClaim;

    public ApplicationRoleClaim ApplicationRoleClaim { get; }
}
