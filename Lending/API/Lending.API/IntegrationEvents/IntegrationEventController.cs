using Dapr;
using Lending.API.IntegrationEvents.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Lending.API.Controller;

[Route("api/v1/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    [HttpPost("OrderStatusChangedToSubmitted")]
    [Topic(DAPR_PUBSUB_NAME, nameof(MyEvent))]
    public Task HandleAsync(
        MyEvent @event,
        [FromServices] Handler handler)
        => handler.Handle(@event);
}
