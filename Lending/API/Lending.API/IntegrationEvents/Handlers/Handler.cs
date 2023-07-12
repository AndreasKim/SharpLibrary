using Lending.Core.Application.Events;
using Lending.Core.Application.Interfaces;

namespace Lending.API.IntegrationEvents.Handlers;

public class Handler
    : IIntegrationEventHandler<MyEvent>
{
    public Task Handle(MyEvent @event)
    {
        Console.WriteLine("RECEIVED");
        Console.WriteLine(@event.OrderId);

        return Task.CompletedTask;
    }
}

public record MyEvent(
    Guid OrderId,
    string OrderStatus,
    string BuyerId)
    : IntegrationEvent;
