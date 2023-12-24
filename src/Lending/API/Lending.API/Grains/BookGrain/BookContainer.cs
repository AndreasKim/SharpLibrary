using Lending.Domain.BookAggregate;

namespace Lending.API.Grains.BookGrain;

[GenerateSerializer, Immutable]
public class BookContainer
{
    public BookContainer()
    {

    }

    public BookContainer(Guid id, Guid libraryBranchId, BookState state, BookType type, HoldLifeType holdLifeTime)
    {
        Book = new Book(id, libraryBranchId, state, type, holdLifeTime);
    }

    public Book? Book { get; set; }
}