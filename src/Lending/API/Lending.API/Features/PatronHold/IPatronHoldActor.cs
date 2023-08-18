namespace Lending.API.Features.PatronHold;

public interface IPatronActor : IGrainWithGuidKey
{
    Task<PatronHoldResponse> PlaceHold(PatronHoldRequest request);
}