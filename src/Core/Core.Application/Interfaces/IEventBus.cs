using Core.Application.Events;

namespace Core.Application.Interfaces;

public interface ITTEventBus
{
    Task PublishAsync(IntegrationEvent integrationEvent);
}
