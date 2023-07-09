using FluentValidation;
using Lending.Domain.BookAggregate;

namespace Lending.Domain.PatronAggregate;

public class Patron
{
    public List<Guid> HoldBookIds { get; } = new List<Guid>();

    public void HoldBook(Book book)
    {
        new PatronValidator(1).ValidateAndThrow(this);

        HoldBookIds.Add(book.Id);
    }
}