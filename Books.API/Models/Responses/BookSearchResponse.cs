using Books.API.Models.DTOs;

namespace Books.API.Models.Responses
{
    /// <summary>
    /// The book search response
    /// </summary>
    public class BookSearchResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Root Result { get; set; }
    }
}
