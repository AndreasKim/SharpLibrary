using FluentValidation;
using Lending.Domain.BookAggregate;
using PatronAggregate.Policies;

namespace Lending.Domain.PatronAggregate;

public class PatronValidator : AbstractValidator<Patron>
{
    public PatronValidator(int booksToHold, Book book)
    {
        Include(new PolicyMaxHold(booksToHold));
        Include(new PolicyOverDueCheckouts(book));
        Include(new PolicyRejectRestricted(book));
        Include(new PolicyRejectOpenEnded(book));
    }
}
