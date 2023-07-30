using Core.Domain;

namespace PatronAggregate.Events;

public record BookPlacedOnHoldEvent : IDomainEvent
{
    private readonly Guid _id;

    public BookPlacedOnHoldEvent(Guid id)
    {
        _id = id;
    }

    public Guid Id => _id;
}