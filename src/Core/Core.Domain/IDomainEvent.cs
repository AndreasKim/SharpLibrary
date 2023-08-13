namespace Core.Domain
{
    public interface IDomainActorEvent
    {
        Guid ActorId { get; }
    }
}