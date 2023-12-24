using AutoFixture.Xunit2;
using FastEndpoints;
using FluentAssertions;
using Lending.API;
using Lending.API.Features.PatronHold;
using Lending.API.Grains.Book;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;

namespace Lending.IntegrationTests
{
    public class PatronHoldTests
    {
        private Repository _repo;

        private readonly WebApplicationFactory<AppSettings> _webApplicationFactory;

        public PatronHoldTests()
        {
            _repo = new Repository("localhost:6379");
            _webApplicationFactory = new WebApplicationFactory<AppSettings>()
                    .WithWebHostBuilder(b =>
                        b.ConfigureServices(s =>
                            s.AddScoped<IRepository>(p => new Repository("localhost:6379"))));
            Client = _webApplicationFactory.CreateClient();
        }

        public HttpClient Client { get; private set; }


        [Theory, AutoData]
        public async Task PatronHoldEndpoint_HoldAvailableBook_Succeeds(PatronHoldRequest request)
        {
            using (var scope = _webApplicationFactory.Services.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IClusterClient>();
                await client.GetGrain<IBookActor>(request.BookId)
                    .Write(new BookContainer(request.BookId, Guid.NewGuid(), BookState.Available, BookType.Circulating, HoldLifeType.CloseEnded));
            }

            await _repo.Upsert(request.BookId, new Book(request.BookId, Guid.NewGuid(), BookState.Available, BookType.Circulating, HoldLifeType.CloseEnded));
            await _repo.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

            var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = request.PatronId, BookId = request.BookId });

            result.Response.Should().NotBeNull();
            result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.OK);
            result.Result.IsSuccess.Should().BeTrue();
            result.Result.ValidationErrors.Should().BeEmpty();
        }

        [Theory, AutoData]
        public async Task PatronHoldEndpoint_HoldUnAvailableBook_ReturnsBadRequest(PatronHoldRequest request)
        {
            await _repo.Upsert(request.BookId, new Book(request.BookId, Guid.NewGuid(), BookState.UnAvailable, BookType.Circulating, HoldLifeType.CloseEnded));
            await _repo.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

            var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = request.PatronId, BookId = request.BookId });

            result.Response.Should().NotBeNull();
            result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.BadRequest);
            result.Result.IsSuccess.Should().BeFalse();
            result.Result.ValidationErrors.Should().NotBeEmpty();
        }

        [Theory, AutoData]
        public async Task PatronHoldEndpoint_HoldNonExistingBook_ReturnsInternalError(PatronHoldRequest request)
        {
            await _repo.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

            var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = request.PatronId, BookId = request.BookId });

            result.Response.Should().NotBeNull();
            result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.InternalServerError);
            result.Result.IsSuccess.Should().BeFalse();
            result.Result.ValidationErrors.Should().BeEmpty();
        }

        [Theory, AutoData]
        public async Task PatronHoldEndpoint_HoldWithEmptyGuid_ReturnsBadRequest(PatronHoldRequest request)
        {
            var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = Guid.Empty, BookId = Guid.Empty });

            result.Response.Should().NotBeNull();
            result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.BadRequest);
            result.Result.IsSuccess.Should().BeFalse();
            result.Result.ValidationErrors.Should().BeEmpty();
        }
    }
}
