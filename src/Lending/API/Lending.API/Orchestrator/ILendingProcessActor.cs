using Dapr.Actors;
using Lending.API.Features.PatronHold;

namespace Lending.API.Orchestrator;

public interface ILendingProcessActor : IActor
{
    Task<PatronHoldResponse> PlaceHold(PatronHoldRequest request);
}