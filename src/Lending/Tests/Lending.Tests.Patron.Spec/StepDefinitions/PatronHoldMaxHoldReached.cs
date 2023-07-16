using System;
using TechTalk.SpecFlow;

namespace PatronAggregate.Spec.StepDefinitions
{
    [Binding]
    public class PatronHoldMaxHoldReached
    {
        private readonly PatronHoldContext _context;

        public PatronHoldMaxHoldReached(PatronHoldContext context)
        {
            _context = context;
        }

        [Then(@"a MaximumHoldsReached Event will be send")]
        public void ThenAMaximumHoldsReachedEventWillBeSend()
        {
            _context.Patron.DomainEvents.Should().NotBeEmpty();
            _context.Patron.DomainEvents.Should().ContainItemsAssignableTo<MaximumHoldsReachedEvent>();
        }
    }
}
