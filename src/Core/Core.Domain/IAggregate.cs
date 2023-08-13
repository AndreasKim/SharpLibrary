namespace Core.Domain
{
    public interface IAggregate
    {
        List<IDomainActorEvent> DomainEvents { get; }
    }
}