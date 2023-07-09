using FluentValidation;
using Lending.Domain.Policies;

namespace Lending.Domain.PatronAggregate;

public class PatronValidator : AbstractValidator<Patron>
{
    public PatronValidator(int booksToHold)
    {
        Include(new PolicyMaxHold(booksToHold));
    }
}
