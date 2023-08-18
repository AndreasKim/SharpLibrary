using Core.Application.Interfaces;
using FluentValidation.Results;
using Lending.API.Features.PatronHold;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;

namespace Lending.API.Orchestrator;

public class PatronActor : Grain, IPatronActor
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
            .Some(p => new PatronHoldResponse() { IsSuccess = p.IsValid, ValidationErrors = p.Errors, StatusCode = p.IsValid ? 200 : 400 })
            .None(() => new PatronHoldResponse() { IsSuccess = false, StatusCode = 500 });

        return result;
    }

    private async Task<ValidationResult> PublishEvents((ValidationResult Validation, Patron Patron) holdResult)
    {
        await Parallel.ForEachAsync(holdResult.Patron.DomainEvents, async (p, c) => await _eventBus.PublishAsync(p));

        return holdResult.Validation;
    }

}
