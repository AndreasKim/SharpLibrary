using System;
using PatronAggregate.Spec.Models;
using TechTalk.SpecFlow;

namespace PatronAggregate.Spec.StepDefinitions
{
    [Binding]
    public class PatronHoldOpenEndedHold
    {
        private readonly PatronHoldContext _context;

        public PatronHoldOpenEndedHold(PatronHoldContext context)
        {
            _context = context;
        }

        [Given(@"an open ended book")]
        public void GivenAnOpenEndedBook()
        {
            _context.Book = new Book(Guid.NewGuid(), Guid.NewGuid(), BookState.Available, BookType.Circulating, HoldLifeType.OpenEnded);
        }
    }
}
