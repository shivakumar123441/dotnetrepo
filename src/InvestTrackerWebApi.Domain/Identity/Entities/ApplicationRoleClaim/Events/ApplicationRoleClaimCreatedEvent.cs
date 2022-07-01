namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRoleClaim.Events;
using InvestTrackerWebApi.Domain.Common;

public class ApplicationRoleClaimCreatedEvent : DomainEvent
{
    public ApplicationRoleClaimCreatedEvent(ApplicationRoleClaim applicationRoleClaim) =>
        this.ApplicationRoleClaim = applicationRoleClaim;

    public ApplicationRoleClaim ApplicationRoleClaim { get; }
}
