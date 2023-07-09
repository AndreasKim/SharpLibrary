using FluentValidation;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;

namespace PatronAggregate.Policies
{
    public class PolicyOverDueCheckouts : AbstractValidator<Patron>
    {
        private const int MAX_CHECKOUT = 2;

        public PolicyOverDueCheckouts(Book book)
        {
            RuleFor(p => p.OverDueCheckouts)
                .Must(p => NotBeLargerThanMaxCheckout(p, book))
                .WithMessage($"More than {MAX_CHECKOUT} books have been checked out.").WithErrorCode("400");
        }

        private static bool NotBeLargerThanMaxCheckout(Dictionary<Guid, int> dictionary, Book book)
        {
            if(dictionary.TryGetValue(book.LibraryBranchId, out var checkouts))
            {
                return checkouts < MAX_CHECKOUT;
            }

            return true;
        }
    }
}