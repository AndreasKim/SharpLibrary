namespace PatronAggregate.Spec.StepDefinitions;

[Binding]
public class PatronHoldAlreadyOnHold
{
    private Patron _patron;
    private Book? _book;
    private Action _holdAction;

    public PatronHoldAlreadyOnHold(Patron patron)
    {
        _patron = patron;
    }


    [Given(@"a book that is already on hold")]
    public void GivenABookThatIsAlreadyOnHold()
    {
        _book = new Book(Guid.NewGuid(), Guid.NewGuid(), BookState.UnAvailable);
    }

    [When(@"the patron tries to hold the book")]
    public void WhenThePatronTriesToHoldTheBook()
    {
        _holdAction = () => _patron.HoldBook(_book);
    }

    [Then(@"the close ended bookhold throws an invalid operation exception\.")]
    public void ThenTheCloseEndedBookholdThrowsAnInvalidOperationException_()
    {
        _holdAction.Should().Throw<InvalidOperationException>();
    }
}
