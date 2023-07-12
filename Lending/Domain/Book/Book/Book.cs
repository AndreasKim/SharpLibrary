namespace Lending.Domain.BookAggregate;

public class Book
{
    public Book(Guid id, Guid libraryBranchId, BookState state, BookType type, HoldLifeType holdLifeTime)
    {
        State = state;
        Id = id;
        LibraryBranchId = libraryBranchId;
        Type = type;
        HoldLifeTime = holdLifeTime;
    }

    public Guid Id { get; private set; }
    public Guid LibraryBranchId { get; private set; }
    public BookState State { get; private set; }
    public BookType Type { get; private set; }
    public HoldLifeType HoldLifeTime { get; private set; }
}