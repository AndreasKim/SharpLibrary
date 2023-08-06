using System.Net;
using Dapr.Actors;
using Dapr.Actors.Client;
using FluentValidation;
using FluentValidation.Results;
using Lending.API.Orchestrator;
using Microsoft.AspNetCore.Mvc;

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

[Route("api/")]
[ApiController]
public class PatronHoldController : ControllerBase
{
    private readonly IValidator<PatronHoldRequest> _validator;

    public PatronHoldController(IValidator<PatronHoldRequest> validator)
    {
        _validator = validator;
    }

    [Route("patronhold")]
    [HttpPost]
    public async Task<IActionResult> HoldBook(PatronHoldRequest request)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var actorId = new ActorId(request.BookId.ToString()); // FIXME: use real BookId class
        var actor = ActorProxy.Create<ILendingProcessActor>(actorId, nameof(LendingProcessActor));

        var result = await actor.PlaceHold(request);

        return Ok();
    }
}