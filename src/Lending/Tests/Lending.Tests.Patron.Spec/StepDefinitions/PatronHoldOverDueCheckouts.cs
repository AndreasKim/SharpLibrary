namespace PatronAggregate.Spec.StepDefinitions;

[Binding]
public class PatronHoldOverDueCheckouts
{
    private readonly PatronHoldContext _context;

    public PatronHoldOverDueCheckouts(PatronHoldContext context)
    {
        _context = context;
    }

    [When(@"the patron has two overdue checkouts at a library branch")]
    public void WhenThePatronHasTwoOverdueCheckoutsAtALibraryBranch()
    {
        var branch = _context.Book.LibraryBranchId;
        _context.Patron.OverDueCheckouts.Add(branch, 2);
    }
}
