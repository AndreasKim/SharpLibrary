using FluentValidation;
using Lending.Domain.PatronAggregate;

namespace PatronAggregate.Policies;

public class PolicyMaxHold : AbstractValidator<Patron>
{
    public PolicyMaxHold(int booksToHold)
    {
        RuleFor(p => p.HoldBookIds.Count + booksToHold).LessThanOrEqualTo(5)
            .WithMessage("Patron can not hold more than 5 books.").WithErrorCode("400");
    }
}
