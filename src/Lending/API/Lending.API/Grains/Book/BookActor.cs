using Orleans.Runtime;

namespace Lending.API.Grains.Book;

public class BookActor : Grain, IBookActor
{
    private readonly IPersistentState<BookContainer> _book;

    public BookActor([PersistentState("book", "libraryStore")] IPersistentState<BookContainer> book)
    {
        _book = book;
    }

    public async Task Write(BookContainer book)
    {
        if (book == _book.State)
        {
            return;
        }

        _book.State = book;
        await _book.WriteStateAsync();
    }

    public Task<BookContainer> Read()
    {
        return Task.FromResult(_book.State);
    }
}
