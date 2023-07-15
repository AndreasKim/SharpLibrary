using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FastEndpoints;
using Lending.API;
using Lending.API.Features.PatronHold;
using Lending.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Lending.IntegrationTests
{
    public class PatronHoldE2ETests
    {
        public static HttpClient Client { get; } = new WebApplicationFactory<AppSettings>()
            .WithWebHostBuilder(b =>
                b.ConfigureServices(s =>
                    s.AddScoped<IRepository, Repository>()))
            .CreateClient();

        [Theory, AutoData]
        public async Task Test(PatronHoldRequest request, Guid PatronId, Guid BookId) 
        {
            var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse >(new() { PatronId = PatronId, BookId = BookId});
            Console.WriteLine();
        }
    }
}
