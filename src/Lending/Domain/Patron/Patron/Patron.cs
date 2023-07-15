using System.Collections.ObjectModel;
using FluentValidation;
using Lending.Domain.BookAggregate;

namespace Lending.Domain.PatronAggregate;

public class Patron
{    
    private readonly List<Guid> _holdBookIds = new();
    public Patron(Guid id, PatronType type)
    {
        Type = type;
        Id = id;
    }

    public Guid Id { get; set; }
    public PatronType Type { get; private set; }
    public ReadOnlyCollection<Guid> HoldBookIds => _holdBookIds.AsReadOnly();
    public Dictionary<Guid, int> OverDueCheckouts { get; private set; } = new();

    public void HoldBook(Book book)
    {
        new PoliciesPatronHold(1, book).ValidateAndThrow(this);

        _holdBookIds.Add(book.Id);
    }
}