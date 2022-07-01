namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;

using InvestTrackerWebApi.Domain.Common;
using Microsoft.AspNetCore.Identity;

public class ApplicationRole : IdentityRole, IAuditableEntity, IHasDomainEvent
{
    public string Description { get; private set; } = string.Empty;
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid LastModifiedBy { get; private set; }
    public DateTime LastModifiedOn { get; private set; }
    public List<DomainEvent> DomainEvents { get; } = new();

    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName, string description, Guid currentUserId)
    : base(roleName)
    {
        this.NormalizedName = roleName.ToUpperInvariant();
        this.Description = description;
        this.CreatedOn = DateTime.UtcNow;
        this.CreatedBy = currentUserId;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
    }

    public void AddDomainEvent(DomainEvent @event) => this.DomainEvents.Add(@event);

    public ApplicationRole Update(string roleName, string description, Guid currentUserId)
    {
        this.Name = roleName;
        this.NormalizedName = roleName.ToUpperInvariant();
        this.Description = description;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        return this;
    }
}
