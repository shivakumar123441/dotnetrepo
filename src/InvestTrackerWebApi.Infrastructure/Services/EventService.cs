namespace InvestTrackerWebApi.Infrastructure.Services;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

public class EventService : IEventService
{
    private readonly ILogger<EventService> logger;
    private readonly IPublisher mediator;

    public EventService(ILogger<EventService> logger, IPublisher mediator)
    {
        this.logger = logger;
        this.mediator = mediator;
    }

    public async Task PublishAsync(DomainEvent @event)
    {
        this.logger.LogInformation("Publishing Event : {event}", @event.GetType().Name);
        await this.mediator.Publish(this.GetEventNotification(@event));
    }

    private INotification GetEventNotification(DomainEvent @event) => (INotification)Activator.CreateInstance(
            typeof(EventNotification<>).MakeGenericType(@event.GetType()), @event)!;
}
