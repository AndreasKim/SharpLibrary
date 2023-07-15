namespace PatronAggregate.Spec.StepDefinitions;

[Binding]
public class PatronHoldSuccess
{
    private readonly PatronHoldContext _context;

    public PatronHoldSuccess(PatronHoldContext context)
    {
        _context = context;
    }

    [When(@"the patron places a hold on the book")]
    public void WhenThePatronPlacesAHoldOnTheBook()
    {
        _context.Patron.HoldBook(_context.Book);
    }

    [Then(@"the close ended bookhold suceeds")]
    public void ThenTheCloseEndedBookholdSuceeds()
    {
        _context.Patron.HoldBookIds.Should().NotBeEmpty();
        _context.Patron.HoldBookIds.Should().Contain(_context.Book.Id);
    }
}
