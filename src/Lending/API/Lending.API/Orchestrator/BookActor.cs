using Lending.Domain.BookAggregate;
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

    public async Task HandleAsync(BookPlacedOnHoldEvent DomainEvent)
    {
        var book = await _repository.Get<Book>(DomainEvent.ActorId);

        var result = book
            .Map(SetOnHold)
            .BindAsync(p => _repository.Upsert(p.Id, p).ToAsync());

       if (await result.IsNone)
       {
            throw new InvalidOperationException($"Was not able to update book {DomainEvent.ActorId}.");
       }
    }

    private Book SetOnHold(Book book)
    {
        book.SetBookOnHold();
        return book;
    }
}
