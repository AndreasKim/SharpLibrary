using FluentValidation;

namespace PatronAggregate.Spec.StepDefinitions;

[Binding]
public class PatronHoldOverMax
{
    private readonly PatronHoldContext _context;

    public PatronHoldOverMax(PatronHoldContext context)
    {
        _context = context;
    }


    [Given(@"a regular patron")]
    public void GivenARegularPatron()
    {
        _context.Patron = new Patron(Guid.NewGuid(), PatronType.Regular);
    }

    [Given(@"an available book")]
    public void GivenAnAvailableBook()
    {
        _context.Book = new Book(Guid.NewGuid(), Guid.NewGuid(), BookState.Available,
            BookType.Circulating, HoldLifeType.CloseEnded);
    }
    

    [When(@"the patron tries to place his (.*)th hold")]
    public void WhenThePatronTriesToPlaceHisThHold(int holds)
    {
        for (int i = 1; i < holds; i++)
        {
            _context.Patron.HoldBook(_context.Book);
        }

        _context.Result = _context.Patron.HoldBook(_context.Book);
    }

    [Then(@"the close ended bookhold fails")]
    public void ThenTheCloseEndedBookholdFails()
    {
        _context.Result.IsValid.Should().BeFalse();
        _context.Result.Errors.Should().NotBeEmpty();
    }
}
