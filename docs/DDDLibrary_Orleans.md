# Actor model with Microsoft Orleans

## Handling domain events

Communication between the aggregates should be handled by domain events. In this way an aggregate can, within the same domain, notify other aggregates about its own changes. In our case, e.g. the Patron can notify about a book being placed on hold. 
For this we can extend the aggregate class every aggregate is inheriting from:

```c#
    public class Aggregate : IAggregate
    {
        public List<IDomainActorEvent> DomainEvents { get; } = new();
    }
``` 

A event can then easily be added as:

```c#
    DomainEvents.Add(new BookPlacedOnHoldEvent(book.Id));
``` 

In the application layer, we can then publish those events to an internal(in process) eventbus, which can look like the following:
```c#
    public async Task<PatronHoldResponse> PlaceHold(PatronHoldRequest request)
    {
        var patron = await _repository.Get<Patron>(request.PatronId);
        var book = await _repository.Get<Book>(request.BookId);

        var holdResult = from p in patron
                         from b in book
                         select (p.HoldBook(b), p);

        var result = await holdResult
            .MapAsync(PublishEvents)
            .MapAsync(CommitPatronChanges)
            .Some(p => new PatronHoldResponse() { IsSuccess = p.IsValid, ValidationErrors = p.Errors, StatusCode = p.IsValid ? 200 : 400 })
            .None(() => new PatronHoldResponse() { IsSuccess = false, StatusCode = 500 });

        return result;
    }

    private async Task<ValidationResult> CommitPatronChanges((ValidationResult Validation, Patron Patron) holdResult)
    {
        await _repository.Upsert(holdResult.Patron.Id, holdResult.Patron);

        return holdResult.Validation;
    }

    private async Task<(ValidationResult Validation, Patron Patron)> PublishEvents((ValidationResult Validation, Patron Patron) holdResult)
    {
        await Parallel.ForEachAsync(holdResult.Patron.DomainEvents, async (p, c) => await _eventBus.PublishAsync(p));

        return holdResult;
    }
``` 

In this context I was thinking a lot about what could go wrong upon notifing other aggregates. For example, when the patron notifies the book about being on hold and something goes wrong within this process it could easily be that we end up in an invalid state. In this case, the patron could be having the book noted on hold, while the book itself is not.

In the context of distributed computing, I found Microsoft Orleans, which can provide an interesting solution.

## Microsoft Orleans

Microsoft Orleans is an open-source framework developed by Microsoft for building distributed, scalable, and reliable applications in .NET. It simplifies the development of large-scale, cloud-based applications by providing a programming model that abstracts away the complexities of distributed systems.

Orleans is built on the actor model, which is a programming paradigm for building concurrent and distributed systems. In Orleans, an actor is a fundamental unit of computation that encapsulates state and behavior.

Each actor is single threaded, which simplifies the programming model, as developers don't have to deal with complex synchronization primitives.

In our case Orleans can be the right fit, especially if we plan our application to expand in a distributed environment later on.

In particular, Orleans actors process messages atomically. When the patron actor notifies the book actor about being on hold, the book actor processes this message sequentially. This ensures that the state transitions within each actor are consistent, reducing the risk of ending up in an invalid state.

Orleans also provides mechanisms for state persistence, allowing the actor state to be stored and recovered in case of failures. This is essential for maintaining the consistency of your system, even in the face of unexpected issues. If something goes wrong during the process of notifying the book, Orleans can recover the state of the book actor to a consistent state.

## Orleans setup

In order to enable Orleans to run, we can simply attach it to our host. For now, I will just add a simple local host cluster, which can later simple be replaced by a silo running on azure.

```c#
builder.Host.UseOrleans(silo =>
    {
        silo.UseLocalhostClustering()
            .ConfigureLogging(logging => logging.AddConsole());
    })
    .UseConsoleLifetime();
``` 

Next we can add our actors, e.g.:

```c#
public class PatronActor : Grain, IPatronHoldActor
{ ...
``` 

where the interface just looks like:

```c#
public interface IPatronHoldActor : IGrainWithGuidKey
{
    Task<PatronHoldResponse> PlaceHold(PatronHoldRequest request);
}
``` 
IGrainWithGuidKey means that we use a Guid to identify our actor, since every actor has a unique key.
The endpoint now refers to our actor via:

```c#
[GenerateSerializer, Immutable]
public record PatronHoldRequest
{
    public Guid PatronId { get; set; }
    public Guid BookId { get; set; }
}

[GenerateSerializer, Immutable]
public record PatronHoldResponse
{
    public List<ValidationFailure> ValidationErrors { get; set; } = new();
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
}

public class PatronHoldEndpoint : Endpoint<PatronHoldRequest>
{
    private readonly IClusterClient _clusterClient;

    public PatronHoldEndpoint(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public override void Configure()
    {
        Post("api/patronhold");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatronHoldRequest request, CancellationToken ct)
    {
        var actor = _clusterClient.GetGrain<IPatronHoldActor>(request.PatronId);

        var result = await actor.PlaceHold(request);

        await SendAsync(result, result.StatusCode, ct);
    }

}
``` 

You may notice the [GenerateSerializer, Immutable] attribute, which is required for every object that is used as input/output for an actor.

## Eventbus

The final piece of the puzzle is the eventbus itself.   
First of all, we want to create a dictionary of all the actors through, that are marked with the IDomainEventHandler attribute and save them in an 'ActorDictionary':

```c#
    public static IServiceCollection AddActorDictionary(this IServiceCollection services)
    {
        var localActorDictionary = Assembly.GetExecutingAssembly().GetTypes()
            .Where(p => typeof(IDomainEventHandler).IsAssignableFrom(p) && p.IsInterface)
            .Distinct()
            .ToDictionary(GetGenericInterfaceArgument, p => p);

        services.AddScoped(p => new ActorDictionary(localActorDictionary));

        return services;
    }
``` 

In the eventbus, we can then use this dictionary, to get the correct actor by type:

```c#
    public Task PublishAsync<T>(T domainEvent) where T : IDomainActorEvent
    {
        var eventType = domainEvent.GetType();
        if (_actorDictionary.TryGetValue(eventType, out var actor))
        {
            var actorImpl = _clusterClient.GetGrain(actor, domainEvent.ActorId);

            var method = this.GetType().GetMethod(nameof(InvokeActor))!;
            var generic = method.MakeGenericMethod(eventType);

            return (Task)generic.Invoke(this, new object[] { actorImpl, domainEvent })!;
        }

        return Task.CompletedTask;
    }

    public static Task InvokeActor<T>(IDomainEventHandler<T> eventHandler, IDomainActorEvent domainActorEvent) where T : IDomainActorEvent
    { 
        return eventHandler.HandleAsync((T)domainActorEvent);
    }
``` 

The InvokeActor<T> method is a little trick, so that we can use generics instead of having to unwrap everything with reflection.

The eventbus now simple calls HandleAsync on the correct actor upon publishing.

In the next round, we will add Orleans persistence and transactions to the mix to have the complete implementation.