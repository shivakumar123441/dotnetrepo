namespace InvestTrackerWebApi.Domain.Common;

public abstract class BaseEntity<T> : IHasDomainEvent
{
    public BaseEntity(T id) => this.Id = id;

    // used by ef core. should not be used for development.
    internal BaseEntity()
    {
    }

    public T? Id { get; private set; }

    public List<DomainEvent> DomainEvents { get; } = new();
}
