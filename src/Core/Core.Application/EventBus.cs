using Core.Application.Events;
using Core.Application.Interfaces;
using Core.Domain;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace Core.Application;

public class EventBus : IEBus
{
    private const string PubSubName = "sharplibrary-pubsub";

    private readonly DaprClient _dapr;
    private readonly ILogger _logger;
    private readonly List<IDomainEventHandler> _handlers;

    public EventBus(DaprClient dapr, ILogger<EventBus> logger, List<IDomainEventHandler> handlers)
    {
        _dapr = dapr;
        _logger = logger;
        _handlers = handlers;
    }

    public async Task PublishAsync(IntegrationEvent integrationEvent)
    {
        var topicName = integrationEvent.GetType().Name;

        _logger.LogInformation(
            "Publishing event {@Event} to {PubsubName}.{TopicName}",
            integrationEvent,
            PubSubName,
            topicName);

        // We need to make sure that we pass the concrete type to PublishEventAsync,
        // which can be accomplished by casting the event to dynamic. This ensures
        // that all event fields are properly serialized.
        await _dapr.PublishEventAsync(PubSubName, topicName, (object)integrationEvent);
    }  
    
    public async Task PublishAsync(IDomainEvent domainEvent)
    {

    }
}
