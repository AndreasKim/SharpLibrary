using Lending.Core.Domain;

namespace Lending.Domain.BookAggregate;

public class BookType : Enumeration
{
    public static BookType Restricted = new(1, nameof(Restricted));
    public static BookType Circulating = new(2, nameof(Circulating));

    public BookType(int id, string name)
        : base(id, name)
    {
    }

}
