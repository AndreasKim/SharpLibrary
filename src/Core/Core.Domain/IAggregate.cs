namespace Core.Domain
{
    public interface IAggregate
    {
        List<IDomainEvent> DomainEvents { get; }
    }
}