using AutoFixture.Xunit2;
using FluentAssertions;
using Lending.Domain.PatronAggregate;

namespace Lending.Infrastructure.UnitTests;

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
        var success = await _repo.Upsert(patron.Id, patron);
        success.IsSome.Should().BeTrue();
        success.Some(p => p.Should().BeTrue());
    }        
    
    [Theory, AutoData]
    public async Task Get(Patron patron)
    {
        var success = await _repo.Upsert(patron.Id, patron);

        var result = await _repo.Get<Patron>(patron.Id);

        success.IsSome.Should().BeTrue();
        success.Some(p => p.Should().BeTrue());
        result.IfSome(p => p.Should().BeEquivalentTo(patron));
    }   
    
    [Theory, AutoData]
    public async Task Upsert_Update(Patron patron, Patron patron2)
    {
        var initial = await _repo.Upsert(patron.Id, patron);
        var final = await _repo.Upsert(patron.Id, patron2);

        var result = await _repo.Get<Patron>(patron.Id);

        initial.IsSome.Should().BeTrue();
        initial.Some(p => p.Should().BeTrue());
        final.IsSome.Should().BeTrue();
        final.Some(p => p.Should().BeTrue());
        result.IfSome(p => p.Should().BeEquivalentTo(patron2));
    }
}