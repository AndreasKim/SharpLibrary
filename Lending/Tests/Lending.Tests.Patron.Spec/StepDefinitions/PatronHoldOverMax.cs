using FluentValidation;

namespace PatronAggregate.Spec.StepDefinitions
{
    [Binding]
    public class PatronHoldOverMax
    {
        private Action? _holdAction;
        private Patron _patron;
        private Book _book;

        public PatronHoldOverMax(Patron patron, Book book)
        {
            _patron = patron;
            _book = book;
        }


        [Given(@"a regular patron")]
        public void GivenARegularPatron()
        {
            _patron = new Patron();
        }

        [Given(@"an available book")]
        public void GivenAnAvailableBook()
        {
            _book = new Book(Guid.NewGuid(), Guid.NewGuid(), BookState.Available);
        }
        

        [When(@"the patron tries to place his (.*)th hold")]
        public void WhenThePatronTriesToPlaceHisThHold(int holds)
        {
            for (int i = 0; i < holds; i++)
            {
                _patron.HoldBook(_book);
            }

            _holdAction = () => _patron.HoldBook(_book);
        }

        [Then(@"the close ended bookhold fails")]
        public void ThenTheCloseEndedBookholdFails()
        {
            _holdAction.Should().Throw<ValidationException>();
        }
    }
}
