namespace Lending.Infrastructure
{
    public interface IRepository
    {
        Task<T?> Get<T>(Guid id);
        Task<bool> Upsert<T>(Guid id, T value);
    }
}