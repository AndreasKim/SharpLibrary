namespace Core.Domain
{
    public class Aggregate : IAggregate
    {
        public List<IDomainEvent> DomainEvents { get; } = new();
    }
}
