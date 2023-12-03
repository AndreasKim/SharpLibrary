# Application Layer with Language Ext

We can now shift our focus to the Application layer and the publication of our first feature PatronHold end-to-end.
At first, some persistence is needed to store books and patrons. In this example I will use Redis as the primary database, mainly since I wanted to
play around with it and see how it will behave in such a usecase while the complexity of this domain is generally limited. In a real usecase you would (likely) not make the same decision, especially when you have certain durability and data recovery requirements.

With Redis in place, it is now possible to handle your realtime data bookstore requirements with sub-ms response times, in case you need it (probably not).  
The Repository will, for now, only contain two methods, Upsert and Get:
```c#
    public async Task<bool> Upsert<T>(Guid id, T value)
    {
        return await _jsonCommand.SetAsync(new RedisKey(id.ToString()), new RedisValue("$"), value);
    }

    public async Task<Option<T>> Get<T>(Guid id)
    {
        var exists = await _db.KeyExistsAsync(new RedisKey(id.ToString()));

        if(exists)
        {
            var result = await _jsonCommand.GetAsync(new RedisKey(id.ToString()));
            var resultStr = JsonSerializer.Deserialize<T>(result.ToString());
            return Some(resultStr);
        }
        else 
            return None;
    }
``` 

You can see that the Get method returns an Option<T>. This is a special class imported from the library LanguageExt.

## LanguageExt
LanguageExt is a library for functional programming in C#. It provides a set of functional data structures, type classes, and utility functions that enable developers to write code in a more functional style.

The LanguageExt library extends the capabilities of C# by adding features such as immutable data structures (like Option, Either, and List), higher-order functions, pattern matching, monads (like Maybe and Result), and other functional programming constructs. These features allow developers to write code that is more expressive, composable, and robust.

With LanguageExt, you can leverage functional programming techniques to write code that is more declarative, easier to reason about, and less prone to errors. It encourages immutability, avoids null references, provides tools for error handling, and promotes the use of pure functions.

In particular, the use of Option<T> allows me to get rid of the usual nullable problem, which will arise when the Id does not exist. 
Instead I can now return a result wrapped in a `Some()` class when there is a result. Otherwise, I return a `None` class.
This will force the None branch to be handled in the upper layers in order to finally unwrap the response.

## FastEndpoints

```c#
public class PatronHoldRequest
{
    public Guid PatronId { get; set; }
    public Guid BookId { get; set; }
}

public class PatronHoldResponse
{
    public List<ValidationFailure> ValidationErrors { get; set; } = new();
    public bool IsSuccess { get; set; }
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

        var validationResult = from p in patron
                               from b in book
                               select p.HoldBook(b);

        var result = validationResult
            .Some(p => new PatronHoldResponse() { IsSuccess = p.IsValid, ValidationErrors = p.Errors })
            .None(() => new PatronHoldResponse() { IsSuccess = false });

        await SendAsync(result, result.IsSuccess ? 200 : 500, ct);
    }
}
```

On the final enpoint I will use FastEndpoints to set up the endpoint in a REPR fashion (Request-Endpoint-Response). Even though opinionated, this library will allow you to set up well performing endpoints that also suprisingly work well with e.g. Dapr. 

Since the repository will now return an Option<T>, we will now need to process their result in a more functional way. For the induction of the method `HoldBook()`, this will look like so:

```c#
        var validationResult = from p in patron
                               from b in book
                               select p.HoldBook(b);
```

With the help of `from` the value of e.g. `patron` will now be unwrapped. If the result is `None`, it will shortcut the result of the whole call to `None`.
The result of the whole call will again be an Option<T>.

Finally, I need to unwrap the result to map it on my response:

```c#
        var result = validationResult
            .Some(p => new PatronHoldResponse() { IsSuccess = p.IsValid, ValidationErrors = p.Errors })
            .None(() => new PatronHoldResponse() { IsSuccess = false });

```
As you can see the `Some` and the `None` case are handled explicitly and depending on the current state of the input, the appropiate response will we created.
