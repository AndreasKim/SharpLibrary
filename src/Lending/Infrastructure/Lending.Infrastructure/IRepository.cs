using LanguageExt;

namespace Lending.Infrastructure
{
    public interface IRepository
    {
        Task<Option<T>> Get<T>(Guid id);
        Task<Option<bool>> Upsert<T>(Guid id, T value);
    }
}