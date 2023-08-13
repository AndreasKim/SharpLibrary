using Core.Domain;

namespace PatronAggregate.Events;

public record MaximumHoldsReachedEvent : IDomainActorEvent
{
    public Guid ActorId => throw new NotImplementedException();
}
