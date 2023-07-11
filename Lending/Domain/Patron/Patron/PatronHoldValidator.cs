using FluentValidation;
using Lending.Domain.BookAggregate;
using PatronAggregate.Policies;

namespace Lending.Domain.PatronAggregate;

public class PatronHoldValidator : AbstractValidator<Patron>
{
    public PatronHoldValidator(int booksToHold, Book book)
    {
        Include(new PolicyMaxHold(booksToHold));
        Include(new PolicyOverDueCheckouts(book));
        Include(new PolicyRejectRestricted(book));
        Include(new PolicyRejectOpenEnded(book));
        Include(new PolicyStateUnavailable(book));
    }
}
