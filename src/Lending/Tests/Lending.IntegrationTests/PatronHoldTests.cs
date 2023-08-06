using System.Net.Http.Json;
using AutoFixture.Xunit2;
using Dapr.Actors;
using Dapr.Actors.Client;
using FastEndpoints;
using FluentAssertions;
using FluentAssertions.Common;
using Lending.API.Features.PatronHold;
using Microsoft.AspNetCore.Hosting;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;
using Man.Dapr.Sidekick;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Dapr.Client;
using Wrapr;
using Microsoft.Extensions.Hosting;

namespace Lending.IntegrationTests
{
    public class PatronHoldTests
    {
        private Repository _repo;

        public PatronHoldTests()
        {
            _repo = new Repository("localhost:6379");
        }

        public static HttpClient Client { get; } = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
               .UseEnvironment(Environments.Production)
               .ConfigureServices(s =>
                {
                    s.AddScoped<IRepository>(p => new Repository("localhost:6379"));
                    s.AddDaprSidekick(p => p.Sidecar =
                        new DaprSidecarOptions()
                        {
                            AppId = "lending",
                            ComponentsDirectory = "D:\\C#\\SharpLibrary\\dapr\\components",
                            ConfigFile = "D:\\C#\\SharpLibrary\\dapr"
                        });
                }))
            .CreateDefaultClient();

        [Theory, AutoData]
        public async Task PatronHoldEndpoint_HoldAvailableBook_Succeeds(PatronHoldRequest request)
        {
            await _repo.Upsert(request.BookId, new Book(request.BookId, Guid.NewGuid(), BookState.Available, BookType.Circulating, HoldLifeType.CloseEnded));
            await _repo.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

            var result = await Client.PostAsJsonAsync("/api/patronhold", request);
            var response = await result.Content.ReadFromJsonAsync<PatronHoldResponse>();

            response.Should().NotBeNull();
            result.Should().HaveStatusCode(System.Net.HttpStatusCode.OK);
            response.IsSuccess.Should().BeTrue();
            response.ValidationErrors.Should().BeEmpty();
        }

        //[Theory, AutoData]
        //public async Task PatronHoldEndpoint_HoldUnAvailableBook_ReturnsBadRequest(PatronHoldRequest request)
        //{
        //    await _repo.Upsert(request.BookId, new Book(request.BookId, Guid.NewGuid(), BookState.UnAvailable, BookType.Circulating, HoldLifeType.CloseEnded));
        //    await _repo.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

        //    var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = request.PatronId, BookId = request.BookId });

        //    result.Response.Should().NotBeNull();
        //    result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.BadRequest);
        //    result.Result.IsSuccess.Should().BeFalse();
        //    result.Result.ValidationErrors.Should().NotBeEmpty();
        //}

        //[Theory, AutoData]
        //public async Task PatronHoldEndpoint_HoldNonExistingBook_ReturnsInternalError(PatronHoldRequest request)
        //{
        //    await _repo.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

        //    var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = request.PatronId, BookId = request.BookId });

        //    result.Response.Should().NotBeNull();
        //    result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.InternalServerError);
        //    result.Result.IsSuccess.Should().BeFalse();
        //    result.Result.ValidationErrors.Should().BeEmpty();
        //}

        //[Theory, AutoData]
        //public async Task PatronHoldEndpoint_HoldWithEmptyGuid_ReturnsBadRequest(PatronHoldRequest request)
        //{
        //    var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = Guid.Empty, BookId = Guid.Empty });

        //    result.Response.Should().NotBeNull();
        //    result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.BadRequest);
        //    result.Result.IsSuccess.Should().BeFalse();
        //    result.Result.ValidationErrors.Should().BeEmpty();
        //}
    }
}
