﻿using Dapr.Client;
using FastEndpoints;
using FluentValidation.Results;
using LanguageExt;
using LanguageExt.Pipes;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;
using NRedisStack.Search;

namespace Lending.API.Features.PatronHold;

public class PatronHoldRequest
{
    public Guid PatronId { get; set; }
    public Guid BookId { get; set; }
}

public class PatronHoldResponse
{
    public List<ValidationFailure> ValidationErrors { get; set; } = new();
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
}

public class PatronHoldEndpoint : Endpoint<PatronHoldRequest>
{
    private readonly IRepository _repository;
    private readonly DaprClient _daprClient;

    public PatronHoldEndpoint(IRepository repository, DaprClient daprClient)
    {
        _repository = repository;
        _daprClient = daprClient;
    }

    public override void Configure()
    {
        Post("api/patronhold");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatronHoldRequest request, CancellationToken ct)
    {
        var patron = await _repository.Get<Patron>(request.PatronId);
        var book = await _repository.Get<Book>(request.BookId);

        var validationResult = from p in patron
                               from b in book
                               select p.HoldBook(b);

        var result = validationResult
            .Some(p => new PatronHoldResponse() { IsSuccess = p.IsValid, ValidationErrors = p.Errors, StatusCode = p.IsValid ? 200 : 400 })
            .None(() => new PatronHoldResponse() { IsSuccess = false, StatusCode = 500 });

        await SendAsync(result, result.StatusCode, ct);
    }
}
