using Lending.Domain.BookAggregate;

namespace Lending.Domain.PatronAggregate;

public class Patron
{
    public List<Guid> BookIds { get; } = new List<Guid>();

    public void HoldBook(Book book)
    {
        throw new NotImplementedException();
    }
}