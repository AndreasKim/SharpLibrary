using Core.Application.Interfaces;
using Dapr.Actors.Runtime;
using PatronAggregate.Events;

namespace Lending.API.Orchestrators;

public class PatronHoldActor : Actor, IDomainEventHandler<BookPlacedOnHoldEvent>
{
    public PatronHoldActor(ActorHost host) : base(host)
    {
    }

    public async Task PlaceHold()
    {

    }
}
