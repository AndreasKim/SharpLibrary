using Core.Application.Interfaces;
using FluentValidation.Results;
using Lending.API.Grains.BookGrain;
using Lending.API.Grains.PatronGrain;
using Lending.Domain.PatronAggregate;
using Orleans.Runtime;
using static LanguageExt.Prelude;

namespace Lending.API.Features.PatronHold;

public class PatronHoldActor : Grain, IPatronHoldActor
{
    private readonly IClusterClient _clusterClient;
    private readonly IEBus _eventBus;

    public PatronHoldActor(IClusterClient client, IEBus eventBus)
    {
        _clusterClient = client;
        _eventBus = eventBus;
    }

    public async Task<PatronHoldResponse> PlaceHold(PatronHoldRequest request)
    {
        var patronActor = await _clusterClient.GetGrain<IPatronActor>(request.PatronId).Read();
        var patron = Some(patronActor.Patron);

        var bookActor = await _clusterClient.GetGrain<IBookActor>(request.BookId).Read();
        var book = Some(bookActor.Book);

        var holdResult = from p in patron
                         from b in book
                         select (p.HoldBook(b), p);

        var result = await holdResult
            .MapAsync(PublishEvents)
            .MapAsync(CommitPatronChanges)
            .Some(p => new PatronHoldResponse() { IsSuccess = p.IsValid, ValidationErrors = p.Errors, StatusCode = p.IsValid ? 200 : 400 })
            .None(() => new PatronHoldResponse() { IsSuccess = false, StatusCode = 500 });

        return result;
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
