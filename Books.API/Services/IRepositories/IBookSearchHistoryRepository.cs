using Books.DataAccess.Entities;

namespace Books.API.Services.IRepositories
{
    public interface IBookSearchHistoryRepository
    {
        Task CreateAsync(BookSearchHistory bookSearchHistory);
    }
}
