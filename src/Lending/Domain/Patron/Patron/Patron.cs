using System.Collections.ObjectModel;
using Core.Domain;
using FluentValidation.Results;
using Lending.Domain.BookAggregate;
using PatronAggregate.Events;

namespace Lending.Domain.PatronAggregate;

public class Patron : Aggregate
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

    public ValidationResult HoldBook(Book book)
    {
       var validationResult = new PoliciesPatronHold(1, book).Validate(this);

        if (validationResult.IsValid)
        {
            _holdBookIds.Add(book.Id);
            DomainEvents.Add(new BookPlacedOnHoldEvent());

            if (HoldBookIds.Count == 5)
                DomainEvents.Add(new MaximumHoldsReachedEvent());
        }

        return validationResult;
    }
}