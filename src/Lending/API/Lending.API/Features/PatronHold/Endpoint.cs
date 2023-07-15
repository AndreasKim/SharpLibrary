using FastEndpoints;
using Lending.Domain.BookAggregate;
using Lending.Domain.PatronAggregate;
using Lending.Infrastructure;

namespace Lending.API.Features.PatronHold;

public class PatronHoldRequest
{
    public Guid PatronId { get; set; }
    public Guid BookId { get; set; }
}

public class PatronHoldResponse
{
    public string FullName { get; set; }
    public bool IsOver18 { get; set; }
}

public class PatronHoldEndpoint : Endpoint<PatronHoldRequest>
{
    private readonly IRepository _repository;

    public PatronHoldEndpoint(IRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("api/patronhold");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatronHoldRequest request, CancellationToken ct)
    {
        var patron = await _repository.Get<Patron>(request.PatronId);
        var book = await _repository.Get<Book>(request.BookId);

        patron.HoldBook(book);

        await SendOkAsync(ct);
    }
}
