using Core.Application.Events;

namespace Core.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent integrationEvent);
}
