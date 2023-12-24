using Core.Application.Interfaces;
using PatronAggregate.Events;

namespace Lending.API.Features.BookHoldEvent;

public interface IBookHoldEventActor : IGrainWithGuidKey, IDomainEventHandler<BookPlacedOnHoldEvent>
{
}
