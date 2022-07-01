namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRoleClaim;

using InvestTrackerWebApi.Domain.Common;
using Microsoft.AspNetCore.Identity;

public class ApplicationRoleClaim : IdentityRoleClaim<string>, IAuditableEntity, IHasDomainEvent
{
    public string Description { get; private set; } = string.Empty;
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid LastModifiedBy { get; private set; }
    public DateTime LastModifiedOn { get; private set; }
    public List<DomainEvent> DomainEvents { get; } = new();

    public ApplicationRoleClaim()
    {
    }

    public ApplicationRoleClaim(string roleClaimDescription, Guid currentUserId)
    {
        this.Description = roleClaimDescription;
        this.CreatedOn = DateTime.UtcNow;
        this.CreatedBy = currentUserId;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
    }

    public void AddDomainEvent(DomainEvent @event) => this.DomainEvents.Add(@event);

    public ApplicationRoleClaim Update(string roleClaimDescription, Guid currentUserId)
    {
        this.Description = roleClaimDescription;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        return this;
    }
}
