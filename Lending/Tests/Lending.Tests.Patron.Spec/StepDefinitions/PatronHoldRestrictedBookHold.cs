using System;
using PatronAggregate.Spec.Models;
using TechTalk.SpecFlow;

namespace PatronAggregate.Spec.StepDefinitions
{
    [Binding]
    public class PatronHoldRestrictedBookHold
    {
        private readonly PatronHoldContext _context;

        public PatronHoldRestrictedBookHold(PatronHoldContext context)
        {
            _context = context;
        }

        [Given(@"a restricted book")]
        public void GivenARestrictedBook()
        {
            _context.Book = new Book(Guid.NewGuid(), Guid.NewGuid(), BookState.Available, BookType.Restricted, HoldLifeType.CloseEnded);
        }
    }
}
