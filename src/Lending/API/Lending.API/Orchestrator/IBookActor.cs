using PatronAggregate.Events;

namespace Lending.API.Orchestrator
{
    public interface IBookActor : IGrainWithGuidKey
    {
        Task HandleAsync(BookPlacedOnHoldEvent DomainEvent);
    }
}