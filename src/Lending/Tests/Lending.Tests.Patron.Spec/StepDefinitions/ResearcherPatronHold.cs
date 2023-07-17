using System;
using TechTalk.SpecFlow;

namespace PatronAggregate.Spec.StepDefinitions
{
    [Binding]
    public class ResearcherPatronHold
    {
        private readonly PatronHoldContext _context;

        public ResearcherPatronHold(PatronHoldContext context)
        {
            _context = context;
        }

        [Given(@"a researcher patron")]
        public void GivenAResearcherPatron()
        {
            _context.Patron = new Patron(Guid.NewGuid(), PatronType.Researcher);
        }
    }
}
