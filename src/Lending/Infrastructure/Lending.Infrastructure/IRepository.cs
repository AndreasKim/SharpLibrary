using LanguageExt;

namespace Lending.Infrastructure
{
    public interface IRepository
    {
        Task<OptionAsync<T>> Get<T>(Guid id);
        Task<OptionAsync<bool>> Upsert<T>(Guid id, T value);
    }
}