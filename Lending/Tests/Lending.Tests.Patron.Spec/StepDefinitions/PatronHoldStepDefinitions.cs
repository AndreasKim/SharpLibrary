using System;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using TechTalk.SpecFlow;

namespace PatronAggregate.Spec.StepDefinitions
{
    [Binding]
    public class PatronHoldStepDefinitions
    {
        Patron? _patron;
        Action? _holdAction;

        [Given(@"a regular patron")]
        public void GivenARegularPatron()
        {
            _patron = new Patron();
        }

        [When(@"the patron tries to place his (.*)th hold")]
        public void WhenThePatronTriesToPlaceHisThHold(int p0)
        {
            var book = new Book();
            _patron.HoldBook(book);
            _patron.HoldBook(book);
            _patron.HoldBook(book);
            _patron.HoldBook(book);
            _holdAction = () => _patron.HoldBook(book);
        }

        [Then(@"the close ended bookhold fails")]
        public void ThenTheCloseEndedBookholdFails()
        {
            _holdAction.Should().Throw<InvalidOperationException>();
        }
    }
}
