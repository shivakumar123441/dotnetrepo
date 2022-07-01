namespace InvestTrackerWebApi.Domain.Common;

public interface ISoftDelete
{
    DateTime? DeletedOn { get; }
    Guid? DeletedBy { get; }
}
