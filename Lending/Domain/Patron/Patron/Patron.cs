using BookAggregate;
using FluentValidation;
using Lending.Domain.BookAggregate;

namespace Lending.Domain.PatronAggregate;

public class Patron
{
    public Patron(PatronType type)
    {
        Type = type;
    }

    public PatronType Type { get; private set; }
    public List<Guid> HoldBookIds { get; } = new();
    public Dictionary<Guid, int> OverDueCheckouts { get; private set; } = new();

    public void HoldBook(Book book)
    {
        if(book.State.IsUnAvailable)
        {
            throw new InvalidOperationException();
        }

        new PatronValidator(1, book).ValidateAndThrow(this);

        HoldBookIds.Add(book.Id);
    }
}