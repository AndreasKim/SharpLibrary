using Lending.Domain.PatronAggregate;

namespace Lending.API.Grains.PatronGrain;

[GenerateSerializer, Immutable]
public class PatronContainer
{
	public PatronContainer()
	{
	}

	public PatronContainer(Guid id, PatronType type)
	{
		Patron = new Patron(id, type);
	}

    public Patron? Patron { get; set; }
}