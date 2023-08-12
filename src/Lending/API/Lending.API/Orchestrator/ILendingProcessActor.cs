using Lending.API.Features.PatronHold;

namespace Lending.API.Orchestrator;

public interface ILendingProcessActor : IGrainWithGuidKey
{
    Task<PatronHoldResponse> PlaceHold(PatronHoldRequest request);
}