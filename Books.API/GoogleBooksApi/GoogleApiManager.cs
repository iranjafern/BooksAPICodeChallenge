using Books.API.Constants;
using Books.API.Extensions;
using Books.API.Models.DTOs;
using Books.API.Models.Requests;
using Books.API.Models.Responses;
using Books.API.Security;
using Books.API.Services.IRepositories;
using Books.DataAccess.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Text;

namespace Books.API.GoogleBooksApi
{
    public class GoogleApiManager : IGoogleApiManager
    {
        private readonly ILogger<IGoogleApiManager> logger;
        private readonly IBookSearchHistoryRepository searchHistoryRepository;
        private readonly IOptions<GoogleBooksAPI> googleBooksAPI;
        private readonly HttpClient _httpClient;
        private BookSearchHistory bookSearchHistoryRecord = new();
        Uri baseAddressUri = new Uri("https://www.googleapis.com/books/v1/");

        public GoogleApiManager(ILogger<IGoogleApiManager> logger, IBookSearchHistoryRepository searchHistoryRepository, IOptions<GoogleBooksAPI> googleBooksAPI)
        {
            this.logger = logger;
            this.searchHistoryRepository = searchHistoryRepository;
            this.googleBooksAPI = googleBooksAPI;
            _httpClient = new HttpClient();            
        }

        /// <summary>
        /// This constructor will be used by the unit testing with the Mocked httpClient
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="searchHistoryRepository"></param>
        /// <param name="googleBooksAPI"></param>
        /// <param name="httpClient">Used for unit testing with the Mocked httpClient</param>
        public GoogleApiManager(ILogger<IGoogleApiManager> logger, IBookSearchHistoryRepository searchHistoryRepository, IOptions<GoogleBooksAPI> googleBooksAPI, HttpClient httpClient)
        {
            this.logger = logger;
            this.searchHistoryRepository = searchHistoryRepository;
            this.googleBooksAPI = googleBooksAPI;
            _httpClient = httpClient;
        }

        public async Task<BookSearchResponse> GetBooksAsync(int startIndex, int pageSize, string query, string userId, string sessionId)
        {
            var output = new BookSearchResponse();
            var searchRequest = new BookSearchRequest
            {
                Query = query,
                StartIndex = startIndex,
                PageSize = pageSize,
                UserInfo = new UserDto
                {
                    UserId = userId,
                    SessionId = sessionId,
                }
            };

            _httpClient.BaseAddress = baseAddressUri;
            _httpClient.DefaultRequestHeaders.Add("api-key", googleBooksAPI.Value.APIKey);

            try
            {
                if (ValidateRequest(searchRequest, output))
                {
                    output = await GetGoogleBooks(searchRequest);                    
                }
                else
                {
                    //Log validation errors
                    logger.LogAppInfo(output.Message, searchRequest.UserInfo);
                }
            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                output.Message = "Invalid Response";
                logger.LogAppError(ex, searchRequest.UserInfo);
            }
            return output;
        }

        public async Task<BookSearchResponse> GetGoogleBooks(BookSearchRequest searchRequest)
        {
            var output = new BookSearchResponse();
            HttpResponseMessage responseMessage;
            
            responseMessage = await _httpClient.GetAsync($"volumes?q={searchRequest.Query}&startIndex={searchRequest.StartIndex}&maxResults={searchRequest.PageSize}");
            responseMessage.EnsureSuccessStatusCode();

            var responseString = await responseMessage.Content.ReadAsStringAsync();

            //Write to a DB table
            await WriteToBookSearchHistory(searchRequest, responseString);

            var rootItem = JsonConvert.DeserializeObject<Root>(responseString);

            if (rootItem != null)
            {
                output.IsSuccess = true;
                output.Result = rootItem;
            }

            return output;
        }

        private bool ValidateRequest(BookSearchRequest searchRequest, BookSearchResponse bookResponse)
        {
            if (searchRequest == null || bookResponse == null)
                return false;

            StringBuilder errors = new StringBuilder();

            if (searchRequest.PageSize > AppConstants.MaxPageSize || searchRequest.PageSize <= 0)
            {
                bookResponse.IsSuccess = false;
                errors.AppendLine($"Given page size is outside the range {0} to {AppConstants.MaxPageSize}");
            }

            if (string.IsNullOrWhiteSpace(searchRequest.Query))
            {
                bookResponse.IsSuccess = false;
                errors.AppendLine("Query cannot be empty");
            }

            bookResponse.Message = errors.ToString();

            return bookResponse.Message.Length == 0;
        }

        private async Task WriteToBookSearchHistory(BookSearchRequest searchRequest, string responseString)
        {            
            bookSearchHistoryRecord.CreatedDataTime = DateTime.Now;
            bookSearchHistoryRecord.SearchQuery = searchRequest.Query;
            bookSearchHistoryRecord.StartIndex = searchRequest.StartIndex;
            bookSearchHistoryRecord.PageSize = searchRequest.PageSize;
            bookSearchHistoryRecord.SearchResult = responseString;
            if (searchRequest.UserInfo != null)
            {
                bookSearchHistoryRecord.SessionId = searchRequest.UserInfo.SessionId;
                bookSearchHistoryRecord.UserId = searchRequest.UserInfo.UserId;
            }            

            try
            {
                await searchHistoryRepository.CreateAsync(bookSearchHistoryRecord);
            }
            catch (Exception ex)
            {
                logger.LogAppError(ex, searchRequest.UserInfo);
            }
        }
    }
}
