using System.Collections.ObjectModel;
using BookAggregate;
using FluentValidation;
using Lending.Domain.BookAggregate;

namespace Lending.Domain.PatronAggregate;

public class Patron
{    
    private readonly List<Guid> _holdBookIds = new();
    public Patron(PatronType type)
    {
        Type = type;
    }

    public PatronType Type { get; private set; }
    public ReadOnlyCollection<Guid> HoldBookIds => _holdBookIds.AsReadOnly();
    public Dictionary<Guid, int> OverDueCheckouts { get; private set; } = new();

    public void HoldBook(Book book)
    {
        new PatronHoldValidator(1, book).ValidateAndThrow(this);

        _holdBookIds.Add(book.Id);
    }
}