namespace InvestTrackerWebApi.Domain.Common;

public abstract class DomainEvent
{
    public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
}
