namespace InvestTrackerWebApi.Domain.Common;

public abstract class AuditableEntity<T> : BaseEntity<T>, IAuditableEntity, ISoftDelete
{
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid LastModifiedBy { get; protected set; }
    public DateTime LastModifiedOn { get; protected set; }
    public DateTime? DeletedOn { get; protected set; }
    public Guid? DeletedBy { get; protected set; }

    // used by ef core. should not be used for development.
    internal AuditableEntity()
    {
    }

    internal AuditableEntity(T id, Guid createdBy, DateTime createdOn, Guid lastModifiedBy, DateTime lastModifiedOn)
        : base(id)
    {
        this.CreatedBy = createdBy;
        this.CreatedOn = createdOn;
        this.LastModifiedBy = lastModifiedBy;
        this.LastModifiedOn = lastModifiedOn;
    }
}
