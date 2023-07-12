using Lending.Core.Domain;

namespace Lending.Domain.PatronAggregate;

public class PatronType : Enumeration
{
    public static PatronType Regular = new(1, nameof(Regular));
    public static PatronType Researcher = new(2, nameof(Researcher));

    public PatronType(int id, string name)
        : base(id, name)
    {
    }

}
