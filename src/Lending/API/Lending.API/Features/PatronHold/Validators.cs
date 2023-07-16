using FastEndpoints;
using FluentValidation;

namespace Lending.API.Features.PatronHold
{
    public class PatronHoldRequestValidator : Validator<PatronHoldRequest>
    {
        public PatronHoldRequestValidator()
        {
            RuleFor(p => p.PatronId).NotNull().NotEqual(Guid.Empty)
                .WithMessage("Patron Id must be a valid Id.");    

            RuleFor(p => p.BookId).NotNull().NotEqual(Guid.Empty)
                .WithMessage("Book Id must be a valid Id.");
        }
    }
}
