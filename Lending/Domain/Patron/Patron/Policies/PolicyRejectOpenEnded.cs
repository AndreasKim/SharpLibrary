using BookAggregate;
using FluentValidation;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;

namespace PatronAggregate.Policies;

public class PolicyRejectOpenEnded : AbstractValidator<Patron>
{
    public PolicyRejectOpenEnded(Book book)
    {
        RuleFor(p => p)
            .Must(p => book.HoldLifeTime == HoldLifeType.CloseEnded)
            .When(p => p.Type == PatronType.Regular)
            .WithMessage("Regular Patron can only hold close ended books.").WithErrorCode("400");
    }
}
