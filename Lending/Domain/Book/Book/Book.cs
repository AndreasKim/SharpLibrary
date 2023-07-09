using BookAggregate;

namespace Lending.Domain.BookAggregate;

public class Book
{
    public Book(Guid id, Guid libraryBranchId, BookState state)
    {
        State = state;
        Id = id;
        LibraryBranchId = libraryBranchId;
    }

    public Guid Id { get; private set; }
    public Guid LibraryBranchId { get; private set; }
    public BookState State { get; private set; }
}