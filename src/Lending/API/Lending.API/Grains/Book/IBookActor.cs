namespace Lending.API.Grains.Book;

public interface IBookActor : IGrainWithGuidKey
{
    Task Write(BookContainer book);
    Task<BookContainer> Read();
}