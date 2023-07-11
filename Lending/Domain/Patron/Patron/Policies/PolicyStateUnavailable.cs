using FluentValidation;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;

namespace PatronAggregate.Policies;

public class PolicyStateUnavailable : AbstractValidator<Patron>
{
    public PolicyStateUnavailable(Book book)
    {
        RuleFor(p => p).Must(p => book.State.IsAvailable)
            .WithMessage("Patron can not hold an unavailable book").WithErrorCode("400");
    }
}
