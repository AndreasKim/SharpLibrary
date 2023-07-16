using AutoFixture.Xunit2;
using FluentAssertions;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;

namespace Lending.UnitTests;

public class RepositoryTests
{
    private Repository _repo;

    public RepositoryTests()
    {
        _repo = new Repository("localhost:6379");
    }

    [Theory, AutoData]
    public async Task Upsert(Patron patron)
    {
        bool success = await _repo.Upsert(patron.Id, patron);
        success.Should().BeTrue();
    }        
    
    [Theory, AutoData]
    public async Task Get(Patron patron)
    {
        bool success = await _repo.Upsert(patron.Id, patron);

        var result = await _repo.Get<Patron>(patron.Id);

        success.Should().BeTrue();
        result.IfSome(p => p.Should().BeEquivalentTo(patron));
    }   
    
    [Theory, AutoData]
    public async Task Upsert_Update(Patron patron, Patron patron2)
    {
        bool initial = await _repo.Upsert(patron.Id, patron);
        bool final = await _repo.Upsert(patron.Id, patron2);

        var result = await _repo.Get<Patron>(patron.Id);

        initial.Should().BeTrue();
        final.Should().BeTrue();
        result.IfSome(p => p.Should().BeEquivalentTo(patron2));
    }
}