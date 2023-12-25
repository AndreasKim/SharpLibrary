using Core.Application.Interfaces;
using FastEndpoints;
using FluentValidation.Results;
using LanguageExt;
using LanguageExt.SomeHelp;
using Lending.API.Grains.BookGrain;
using Lending.API.Grains.PatronGrain;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using static LanguageExt.Prelude;

namespace Lending.API.Features.PatronHold;

[GenerateSerializer, Immutable]
public record PatronHoldRequest
{
    public Guid PatronId { get; set; }
    public Guid BookId { get; set; }
}

[GenerateSerializer, Immutable]
public record PatronHoldResponse
{
    public List<ValidationFailure> ValidationErrors { get; set; } = new();
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
}

public class PatronHoldEndpoint : Endpoint<PatronHoldRequest>
{
    private readonly IClusterClient _clusterClient;
    private readonly IEBus _eventBus;

    public PatronHoldEndpoint(IClusterClient clusterClient, IEBus eventBus)
    {
        _clusterClient = clusterClient;
        _eventBus = eventBus;
    }

    public override void Configure()
    {
        Post("api/patronhold");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatronHoldRequest request, CancellationToken ct)
    {
        var patronActor = await _clusterClient.GetGrain<IPatronActor>(request.PatronId).Read();
        Option<Patron> patron = patronActor.Patron;

        var bookActor = await _clusterClient.GetGrain<IBookActor>(request.BookId).Read();
        Option<Book> book = bookActor.Book;

        var holdResult = from p in patron
                         from b in book
                         select (p.HoldBook(b), p);

        var result = await holdResult
            .MapAsync(PublishEvents)
            .MapAsync(CommitPatronChanges)
            .Some(p => new PatronHoldResponse() { IsSuccess = p.IsValid, ValidationErrors = p.Errors, StatusCode = p.IsValid ? 200 : 400 })
            .None(() => new PatronHoldResponse() { IsSuccess = false, StatusCode = 500 });

        await SendAsync(result, result.StatusCode, ct);
    }

    private async Task<ValidationResult> CommitPatronChanges((ValidationResult Validation, Patron Patron) holdResult)
    {
        var container = new PatronContainer { Patron = holdResult.Patron };

        await _clusterClient
            .GetGrain<IPatronActor>(holdResult.Patron.Id)
            .Write(container);

        return holdResult.Validation;
    }

    private async Task<(ValidationResult Validation, Patron Patron)> PublishEvents((ValidationResult Validation, Patron Patron) holdResult)
    {
        await Parallel.ForEachAsync(holdResult.Patron.DomainEvents, async (p, c) => await _eventBus.PublishAsync(p));

        return holdResult;
    }
}
