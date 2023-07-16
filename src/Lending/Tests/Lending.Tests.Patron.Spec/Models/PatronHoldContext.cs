using FluentValidation.Results;

namespace PatronAggregate.Spec.Models;

public class PatronHoldContext
{
    public Book Book { get; set; }
    public Patron Patron { get; set; }
    public ValidationResult Result { get; set; }
}
