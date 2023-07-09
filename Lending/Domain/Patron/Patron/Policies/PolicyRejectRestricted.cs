using BookAggregate;
using FluentValidation;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;

namespace PatronAggregate.Policies;

public class PolicyRejectRestricted : AbstractValidator<Patron>
{
    public PolicyRejectRestricted(Book book)
    {
        RuleFor(p => p)
            .Must(p => book.Type == BookType.Circulating)
            .When(p => p.Type == PatronType.Regular)
            .WithMessage("Regular Patron can only hold circulating books.").WithErrorCode("400");
    }
}
