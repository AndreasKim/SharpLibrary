using Dapr.Actors;
using Dapr.Actors.Client;
using FastEndpoints;
using FluentValidation.Results;
using LanguageExt;
using Lending.API.Orchestrator;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;

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
    private readonly IActorProxyFactory _actorProxyFactory;

    public PatronHoldEndpoint(IRepository repository, IActorProxyFactory actorProxyFactory)
    {
        _repository = repository;
        _actorProxyFactory = actorProxyFactory;
    }

    public override void Configure()
    {
        Post("api/patronhold");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatronHoldRequest request, CancellationToken ct)
    {
        var actorId = new ActorId(request.BookId.ToString()); // FIXME: use real BookId class
        var actor = _actorProxyFactory.CreateActorProxy<ILendingProcessActor>(
            actorId,
            nameof(ILendingProcessActor));

        var result = await actor.PlaceHold(request);

        await SendAsync(result, result.StatusCode, ct);
    }

}
