using Core.Application.Interfaces;
using Lending.Infrastructure;
using PatronAggregate.Events;

namespace Lending.API.Orchestrator;

public class TestActor : Grain, IBookActor, IDomainEventHandler
{
    private readonly IRepository _repository;

    public TestActor(IRepository repository)
    {
        _repository = repository;
    }

    public Type? GetActorInterface() => typeof(IBookActor);

    public Task HandleAsync(BookPlacedOnHoldEvent DomainEvent)
    {
        throw new NotImplementedException();
    }

}
