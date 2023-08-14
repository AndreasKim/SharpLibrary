using Core.Domain;

namespace PatronAggregate.Events;

[GenerateSerializer, Immutable]
public record BookPlacedOnHoldEvent : IDomainActorEvent
{
    private readonly Guid _id;

    public BookPlacedOnHoldEvent(Guid id)
    {
        _id = id;
    }

    public Guid ActorId => _id;
}