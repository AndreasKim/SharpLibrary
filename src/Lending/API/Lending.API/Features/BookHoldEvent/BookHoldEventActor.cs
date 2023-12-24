using LanguageExt;
using Lending.API.Grains.Book;
using PatronAggregate.Events;
using static LanguageExt.Prelude;

namespace Lending.API.Features.BookHoldEvent;

public class BookHoldEventActor : Grain, IBookHoldEventActor
{
    private readonly IClusterClient _clusterClient;

    public BookHoldEventActor(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public Type? GetActorInterface() => typeof(IBookHoldEventActor);

    public async Task HandleAsync(BookPlacedOnHoldEvent DomainEvent)
    {
        var book = GetBook(DomainEvent);

        await book
            .Map(SetOnHold)
            .Bind(UpdateBook)
            .IfFailThrow();
    }

    private TryAsync<BookContainer> UpdateBook(BookContainer book)
        => TryAsync(async () =>
        {
            await _clusterClient
                .GetGrain<IBookActor>(book.Book.Id)
                .Write(book);
            return book;
        });

    private TryAsync<BookContainer> GetBook(BookPlacedOnHoldEvent DomainEvent)
        => TryAsync(() => _clusterClient
                .GetGrain<IBookActor>(DomainEvent.ActorId)
                .Read());

    private BookContainer SetOnHold(BookContainer book)
    {
        book.Book.SetBookOnHold();
        return book;
    }
}
