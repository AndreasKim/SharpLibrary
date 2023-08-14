using Core.Domain;

namespace Core.Application.Interfaces
{
    public interface IDomainEventHandler<T> : IDomainEventHandler where T : IDomainActorEvent
    {
        Task HandleAsync(T DomainEvent);
    }    
    
    public interface IDomainEventHandler
    {
    }
}