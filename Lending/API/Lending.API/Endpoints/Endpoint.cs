using Dapr;
using FastEndpoints;

namespace Lending.API.Endpoints;

public class MyRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}

public class MyResponse
{
    public string FullName { get; set; }
    public bool IsOver18 { get; set; }
}

public class MyEndpoint : Endpoint<MyRequest>
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    public override void Configure()
    {
        Post("/api/myevent");
        AllowAnonymous();
    }

    [Topic(DAPR_PUBSUB_NAME, "MyEvent")]
    public override async Task HandleAsync(MyRequest req, CancellationToken ct)
    {
        Console.WriteLine($"Hello world {req.FirstName}");

        var response = new MyResponse()
        {
            FullName = req.FirstName + " " + req.LastName,
            IsOver18 = req.Age > 18
        };

        await SendAsync(response, cancellation: ct);
    }
}
