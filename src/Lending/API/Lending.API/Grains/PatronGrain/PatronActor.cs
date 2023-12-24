using Orleans.Runtime;

namespace Lending.API.Grains.PatronGrain;

public class PatronActor : Grain, IPatronActor
{
    private readonly IPersistentState<PatronContainer> _patron;

    public PatronActor([PersistentState("patron", "libraryStore")] IPersistentState<PatronContainer> patron)
    {
        _patron = patron;
    }

    public async Task Write(PatronContainer patron)
    {
        if (patron == _patron.State)
        {
            return;
        }

        _patron.State = patron;
        await _patron.WriteStateAsync();
    }

    public Task<PatronContainer> Read()
    {
        return Task.FromResult(_patron.State);
    }
}
