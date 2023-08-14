using Lending.Infrastructure;
using PatronAggregate.Events;

namespace Lending.API.Orchestrator;

public class BookActor : Grain, IBookActor
{
    private readonly IRepository _repository;

    public BookActor(IRepository repository)
    {
        _repository = repository;
    }

    public Type? GetActorInterface() => typeof(IBookActor);

    public Task HandleAsync(BookPlacedOnHoldEvent DomainEvent)
    {
        throw new NotImplementedException();
    }

}
