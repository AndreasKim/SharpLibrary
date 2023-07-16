﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FastEndpoints;
using FluentAssertions;
using Lending.API;
using Lending.API.Features.PatronHold;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema.Validation.FormatValidators;

namespace Lending.IntegrationTests
{
    public class PatronHoldTests
    {
        public static HttpClient Client { get; } = new WebApplicationFactory<AppSettings>()
            .WithWebHostBuilder(b =>
                b.ConfigureServices(s =>
                    s.AddScoped<IRepository, Repository>()))
            .CreateClient();

        [Theory, AutoData]
        public async Task PatronHoldEndpoint_HoldAvailableBook_Succeeds(PatronHoldRequest request)
        {
            var repository = new Repository();
            await repository.Upsert(request.BookId, new Book(request.BookId, Guid.NewGuid(), BookState.Available, BookType.Circulating, HoldLifeType.CloseEnded));
            await repository.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

            var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = request.PatronId, BookId = request.BookId });

            result.Response.Should().NotBeNull();
            result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.OK);
            result.Result.IsSuccess.Should().BeTrue();
            result.Result.ValidationErrors.Should().BeEmpty();
        }

        [Theory, AutoData]
        public async Task PatronHoldEndpoint_HoldUnAvailableBook_ReturnsValidationError(PatronHoldRequest request)
        {
            var repository = new Repository();
            await repository.Upsert(request.BookId, new Book(request.BookId, Guid.NewGuid(), BookState.UnAvailable, BookType.Circulating, HoldLifeType.CloseEnded));
            await repository.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

            var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = request.PatronId, BookId = request.BookId });

            result.Response.Should().NotBeNull();
            result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.InternalServerError);
            result.Result.IsSuccess.Should().BeFalse();
            result.Result.ValidationErrors.Should().NotBeEmpty();
        }

        [Theory, AutoData]
        public async Task PatronHoldEndpoint_HoldNonExistingBook_ReturnsInternalError(PatronHoldRequest request)
        {
            var repository = new Repository();
            await repository.Upsert(request.PatronId, new Patron(request.PatronId, PatronType.Regular));

            var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = request.PatronId, BookId = request.BookId });

            result.Response.Should().NotBeNull();
            result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.InternalServerError);
            result.Result.IsSuccess.Should().BeFalse();
            result.Result.ValidationErrors.Should().BeEmpty();
        }

        [Theory, AutoData]
        public async Task PatronHoldEndpoint_HoldWithEmptyGuid_ReturnsInternalError(PatronHoldRequest request)
        {
            var result = await Client.POSTAsync<PatronHoldEndpoint, PatronHoldRequest, PatronHoldResponse>(new() { PatronId = Guid.Empty, BookId = Guid.Empty });

            result.Response.Should().NotBeNull();
            result.Response.Should().HaveStatusCode(System.Net.HttpStatusCode.BadRequest);
            result.Result.IsSuccess.Should().BeFalse();
            result.Result.ValidationErrors.Should().BeEmpty();
        }
    }
}
