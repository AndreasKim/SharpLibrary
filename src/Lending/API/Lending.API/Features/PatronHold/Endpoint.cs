using Core.Domain;
using FastEndpoints;
using FluentValidation.Results;
using Lending.API.Orchestrator;

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

    public PatronHoldEndpoint(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public override void Configure()
    {
        Post("api/patronhold");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatronHoldRequest request, CancellationToken ct)
    {
        var actor = _clusterClient.GetGrain<ILendingProcessActor>(request.PatronId);

        var result = await actor.PlaceHold(request);

        await SendAsync(result, result.StatusCode, ct);
    }

}
