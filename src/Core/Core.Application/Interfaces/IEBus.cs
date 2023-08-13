using Core.Application.Events;
using Core.Domain;

namespace Core.Application.Interfaces;

public interface IEBus
{
    Task PublishAsync(IntegrationEvent integrationEvent);
    Task PublishAsync<T>(T domainEvent) where T : IDomainActorEvent;
}
