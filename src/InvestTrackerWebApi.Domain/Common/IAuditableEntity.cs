namespace InvestTrackerWebApi.Domain.Common;

public interface IAuditableEntity
{
    public Guid CreatedBy { get; }
    public DateTime CreatedOn { get; }
    public Guid LastModifiedBy { get; }
    public DateTime LastModifiedOn { get; }
}
