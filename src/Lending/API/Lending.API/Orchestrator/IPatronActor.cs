using Lending.API.Features.PatronHold;

namespace Lending.API.Orchestrator;

public interface IPatronActor : IGrainWithGuidKey
{
    Task<PatronHoldResponse> PlaceHold(PatronHoldRequest request);
}