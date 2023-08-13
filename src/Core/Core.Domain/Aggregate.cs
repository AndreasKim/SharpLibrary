namespace Core.Domain
{
    public class Aggregate : IAggregate
    {
        public List<IDomainActorEvent> DomainEvents { get; } = new();
    }
}
