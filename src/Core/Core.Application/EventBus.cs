using System.Reflection;
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

    public async Task PublishAsync<T>(T domainEvent) where T : IDomainActorEvent
    {
        var eventType = domainEvent.GetType();
        if (_actorDictionary.TryGetValue(eventType, out var actor))
        {
            var method = this.GetType().GetMethod(nameof(InvokeActor))!;
            var generic = method.MakeGenericMethod(actor);
            var test = generic.Invoke(this, new object[] { domainEvent.ActorId });

            //dynamic actorImpl = _clusterClient.GetGrain(actor, domainEvent.ActorId);
            //var cast = (IDomainEventHandler<BookPlacedOnHoldEvent, IBookActor>)actorImpl;
            Console.WriteLine();
        }
    }

    public async Task InvokeActor<T>(Guid ActorId) where T : IGrainWithGuidKey
    { 
        var grain = _clusterClient.GetGrain<T>(ActorId);
        Console.WriteLine();
    }
}