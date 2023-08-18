namespace Lending.API.Features.PatronHold;

public interface IPatronHoldActor : IGrainWithGuidKey
{
    Task<PatronHoldResponse> PlaceHold(PatronHoldRequest request);
}