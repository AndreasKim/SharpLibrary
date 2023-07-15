using FastEndpoints;

namespace Lending.API.Features.PatronHold;

public class PatronHoldRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}

public class PatronHoldResponse
{
    public string FullName { get; set; }
    public bool IsOver18 { get; set; }
}

public class PatronHoldEndpoint : Endpoint<PatronHoldRequest>
{
    public override void Configure()
    {
        Post("myevent");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatronHoldRequest req, CancellationToken ct)
    {
        Console.WriteLine($"Hello world {req.FirstName}");

        var response = new PatronHoldResponse()
        {
            FullName = req.FirstName + " " + req.LastName,
            IsOver18 = req.Age > 18
        };

        await SendAsync(response, cancellation: ct);
    }
}
