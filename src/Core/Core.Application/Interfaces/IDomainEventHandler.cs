namespace Core.Application.Interfaces
{
    public interface IDomainEventHandler<T, A> : IDomainEventHandler where T : class
    {
        Type? GetActorInterface();
        Task HandleAsync(T DomainEvent);
    }    
    
    public interface IDomainEventHandler
    {
    }
}