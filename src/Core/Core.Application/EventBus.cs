using System.Reflection;
using Core.Application.Events;
using Core.Application.Interfaces;
using Core.Domain;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Core.Application;

public class EventBus : IEBus
{
    private const string PubSubName = "sharplibrary-pubsub";

    private readonly DaprClient _dapr;
    private readonly ILogger _logger;
    private readonly ActorDictionary _actorDictionary;
    private readonly IClusterClient _clusterClient;

    public EventBus(DaprClient dapr, ILogger<EventBus> logger, ActorDictionary actorDictionary, IClusterClient clusterClient)
    {
        _dapr = dapr;
        _logger = logger;
        _actorDictionary = actorDictionary;
        _clusterClient = clusterClient;
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

    public Task PublishAsync<T>(T domainEvent) where T : IDomainActorEvent
    {
        var eventType = domainEvent.GetType();
        if (_actorDictionary.TryGetValue(eventType, out var actor))
        {
            var actorImpl = _clusterClient.GetGrain(actor, domainEvent.ActorId);

            var method = this.GetType().GetMethod(nameof(InvokeActor))!;
            var generic = method.MakeGenericMethod(eventType);

            return (Task)generic.Invoke(this, new object[] { actorImpl, domainEvent })!;
        }

        return Task.CompletedTask;
    }

    public static Task InvokeActor<T>(IDomainEventHandler<T> eventHandler, IDomainActorEvent domainActorEvent) where T : IDomainActorEvent
    { 
        return eventHandler.HandleAsync((T)domainActorEvent);
    }
}