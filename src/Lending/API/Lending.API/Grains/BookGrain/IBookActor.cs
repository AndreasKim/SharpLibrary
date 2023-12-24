namespace Lending.API.Grains.BookGrain;

public interface IBookActor : ICacheActor<BookContainer>, IGrainWithGuidKey
{
}