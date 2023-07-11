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
        _context.Book = new Book(Guid.NewGuid(), Guid.NewGuid(), BookState.UnAvailable,
            BookType.Circulating, HoldLifeType.CloseEnded);
    }

    [When(@"the patron tries to hold the book")]
    public void WhenThePatronTriesToHoldTheBook()
    {
        _context.HoldAction = () => _context.Patron.HoldBook(_context.Book);
    }
}
