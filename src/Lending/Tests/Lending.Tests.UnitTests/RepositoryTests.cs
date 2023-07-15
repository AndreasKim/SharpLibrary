using AutoFixture.Xunit2;
using FluentAssertions;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;

namespace Lending.UnitTests;

public class RepositoryTests
{
    [Theory, AutoData]
    public async Task Upsert(Patron patron, Repository repo)
    {
        bool success = await repo.Upsert(patron.Id, patron);
        success.Should().BeTrue();
    }        
    
    [Theory, AutoData]
    public async Task Get(Patron patron, Repository repo)
    {
        bool success = await repo.Upsert(patron.Id, patron);

        var result = await repo.Get<Patron>(patron.Id);

        success.Should().BeTrue();
        result.Should().BeEquivalentTo(patron);
    }
}