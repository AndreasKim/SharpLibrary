using FluentValidation;
using Lending.Domain.BookAggregate;

namespace Lending.Domain.PatronAggregate;

public class PoliciesPatronHold : AbstractValidator<Patron>
{    
    private const int MAX_CHECKOUT = 2;

    public PoliciesPatronHold(int booksToHold, Book book)
    {
        PolicyMaxHold(booksToHold);
        PolicyOverDueCheckouts(book);
        PolicyRejectRestricted(book);
        PolicyRejectOpenEnded(book);
        PolicyStateUnavailable(book);
    }

    public void PolicyMaxHold(int booksToHold)
    {
        RuleFor(p => p.HoldBookIds.Count + booksToHold).LessThanOrEqualTo(5)
            .WithMessage("Regular Patron can not hold more than 5 books.").WithErrorCode("400");
    }

    public void PolicyRejectOpenEnded(Book book)
    {
        RuleFor(p => p)
            .Must(p => book.HoldLifeTime == HoldLifeType.CloseEnded)
            .When(p => p.Type == PatronType.Regular)
            .WithMessage("Regular Patron can only hold close ended books.").WithErrorCode("400");
    }

    public void PolicyStateUnavailable(Book book)
    {
        RuleFor(p => p).Must(p => book.State.IsAvailable)
            .WithMessage("Patron can not hold an unavailable book").WithErrorCode("400");
    }

    public void PolicyRejectRestricted(Book book)
    {
        RuleFor(p => p)
            .Must(p => book.Type == BookType.Circulating)
            .When(p => p.Type == PatronType.Regular)
            .WithMessage("Regular Patron can only hold circulating books.").WithErrorCode("400");
    }

    public void PolicyOverDueCheckouts(Book book)
    {
        RuleFor(p => p.OverDueCheckouts)
            .Must(p => NotBeLargerThanMaxCheckout(p, book))
            .WithMessage($"More than {MAX_CHECKOUT} books have been checked out.").WithErrorCode("400");
    }

    private static bool NotBeLargerThanMaxCheckout(Dictionary<Guid, int> dictionary, Book book)
    {
        if (dictionary.TryGetValue(book.LibraryBranchId, out var checkouts))
        {
            return checkouts < MAX_CHECKOUT;
        }

        return true;
    }
}
