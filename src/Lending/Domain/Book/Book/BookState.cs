using Lending.Core.Domain;

namespace Lending.Domain.BookAggregate;


public class BookState : Enumeration
{
    public static BookState Available = new(1, nameof(Available));
    public static BookState UnAvailable = new(2, nameof(UnAvailable));

    public BookState(int id, string name)
        : base(id, name)
    {
    }

    public bool IsAvailable => Name == nameof(Available);
    public bool IsUnAvailable => Name != nameof(Available);
}
