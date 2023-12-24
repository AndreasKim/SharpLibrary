namespace Lending.API.Grains
{
    public interface ICacheActor<T>
    {
        Task<T> Read();
        Task Write(T book);
    }
}