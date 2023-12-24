using Lending.API.Grains.BookGrain;
using PatronAggregate.Events;

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
        var bookActor = _clusterClient.GetGrain<IBookActor>(DomainEvent.ActorId);
        var book = await bookActor.Read();

        book.Book?.SetBookOnHold();

        await bookActor.Write(book);
    }
}
