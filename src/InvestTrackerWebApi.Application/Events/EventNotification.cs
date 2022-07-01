namespace InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Domain.Common;
using MediatR;

public class EventNotification<T> : INotification
where T : DomainEvent
{
    public EventNotification(T domainEvent) => this.DomainEvent = domainEvent;

    public T DomainEvent { get; }
}
