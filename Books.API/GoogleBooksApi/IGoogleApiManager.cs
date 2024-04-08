using Books.API.Models.Requests;
using Books.API.Models.Responses;

namespace Books.API.GoogleBooksApi
{
    public interface IGoogleApiManager
    {
        Task<BookSearchResponse> GetBooksAsync(int startIndex, int pageSize, string query, string userId, string sessionId);
    }
}
