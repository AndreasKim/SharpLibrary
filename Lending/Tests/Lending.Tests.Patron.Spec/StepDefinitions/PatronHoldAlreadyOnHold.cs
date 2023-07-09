using PatronAggregate.Spec.Models;

namespace PatronAggregate.Spec.StepDefinitions;

[Binding]
public class PatronHoldAlreadyOnHold
{
    private readonly PatronHoldContext _context;

    public PatronHoldAlreadyOnHold(PatronHoldContext context)
    {
        _context = context;
    }


    [Given(@"a book that is already on hold")]
    public void GivenABookThatIsAlreadyOnHold()
    {
        _context.Book = new Book(Guid.NewGuid(), Guid.NewGuid(), BookState.UnAvailable);
    }

    [When(@"the patron tries to hold the book")]
    public void WhenThePatronTriesToHoldTheBook()
    {
        _context.HoldAction = () => _context.Patron.HoldBook(_context.Book);
    }

    [Then(@"the close ended bookhold throws an invalid operation exception\.")]
    public void ThenTheCloseEndedBookholdThrowsAnInvalidOperationException_()
    {
        _context.HoldAction.Should().Throw<InvalidOperationException>();
    }
}
