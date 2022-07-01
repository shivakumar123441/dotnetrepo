namespace InvestTrackerWebApi.Application.Events;

using InvestTrackerWebApi.Domain.Common;

public interface IEventService
{
    Task PublishAsync(DomainEvent domainEvent);
}
