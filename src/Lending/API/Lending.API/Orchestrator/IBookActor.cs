using Core.Application.Interfaces;
using PatronAggregate.Events;

namespace Lending.API.Orchestrator;

public interface IBookActor : IGrainWithGuidKey, IDomainEventHandler<BookPlacedOnHoldEvent>
{
}
