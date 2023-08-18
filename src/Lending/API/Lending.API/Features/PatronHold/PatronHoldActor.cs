using Core.Application.Interfaces;
using FluentValidation.Results;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;

namespace Lending.API.Features.PatronHold;

public class PatronActor : Grain, IPatronHoldActor
{
    private readonly IRepository _repository;
    private readonly IEBus _eventBus;

    public PatronActor(IRepository repository, IEBus eventBus)
    {
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<PatronHoldResponse> PlaceHold(PatronHoldRequest request)
    {
        var patron = await _repository.Get<Patron>(request.PatronId);
        var book = await _repository.Get<Book>(request.BookId);

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
        await _repository.Upsert(holdResult.Patron.Id, holdResult.Patron);

        return holdResult.Validation;
    }

    private async Task<(ValidationResult Validation, Patron Patron)> PublishEvents((ValidationResult Validation, Patron Patron) holdResult)
    {
        await Parallel.ForEachAsync(holdResult.Patron.DomainEvents, async (p, c) => await _eventBus.PublishAsync(p));

        return holdResult;
    }

}
