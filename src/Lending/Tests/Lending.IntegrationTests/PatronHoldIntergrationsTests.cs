using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FastEndpoints;
using Lending.API.Features.PatronHold;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;

namespace Lending.IntegrationTests
{
    public class PatronHoldIntergrationsTests
    {

        [Theory, AutoData]
        public async Task Test(PatronHoldRequest request)
        {
            var repository = new Repository();
            await repository.Upsert(request.BookId, new Book(request.BookId, Guid.NewGuid(), BookState.Available, BookType.Circulating, HoldLifeType.CloseEnded));
            await repository.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

            var endpoint = Factory.Create<PatronHoldEndpoint>(repository);
            await endpoint.HandleAsync(request, CancellationToken.None);

            Console.WriteLine();
        }
    }
}
