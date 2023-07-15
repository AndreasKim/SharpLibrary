using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Lending.API.Features.PatronHold;
using Lending.Infrastructure;

namespace Lending.IntegrationTests
{
    public class PatronHoldIntergrationsTests
    {

        [Theory, AutoData]
        public async Task Test(PatronHoldRequest request)
        {
            var endpoint = new PatronHoldEndpoint(new Repository());
            await endpoint.HandleAsync(request, default);
            Console.WriteLine();
        }
    }
}
