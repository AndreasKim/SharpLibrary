using Core.Domain;
using PatronAggregate.Events;

namespace Lending.API.Orchestrator;

public interface IBookActor : IGrainWithGuidKey
{
    Task HandleAsync(BookPlacedOnHoldEvent DomainEvent);
}


//[GenerateSerializer, Immutable]
//public record BookPlacedOnHoldEventDto : IDomainActorEvent
//{
//    private readonly Guid _id;

//    public BookPlacedOnHoldEventDto(Guid id)
//    {
//        _id = id;
//    }

//    public Guid ActorId => _id;

//    public static implicit operator BookPlacedOnHoldEventDto(BookPlacedOnHoldEvent input) => new (input.ActorId);
//}