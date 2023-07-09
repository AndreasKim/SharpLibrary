using System;
using TechTalk.SpecFlow;

namespace PatronAggregate.Spec.StepDefinitions
{
    [Binding]
    public class PatronHoldOverDueCheckouts
    {
        private readonly Patron _patron;
        private readonly Book _book;

        public PatronHoldOverDueCheckouts(Patron patron, Book book)
        {
            _patron = patron;
            _book = book;
        }

        [When(@"the patron has two overdue checkouts at a library branch")]
        public void WhenThePatronHasTwoOverdueCheckoutsAtALibraryBranch()
        {
            var branch = _book.LibraryBranchId;
            _patron.OverDueCheckouts.Add(branch, 2);
        }
    }
}
