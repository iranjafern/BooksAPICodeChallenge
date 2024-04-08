using Books.API.GoogleBooksApi;
using Books.API.Models.DTOs;
using Books.API.Models.Responses;
using Books.API.Security;
using Books.API.Services.IRepositories;
using Books.DataAccess.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Books.API.Models.Requests;
using Books.Test.GoogleBooksApi;

namespace Books.Test.Controllers
{
    [TestFixture]
    public class GoogleApiManagerTest
    {
        [Test]
        public void GetGoogleBooksSuccessTest()
        {
            var bookSearchRequest = Mock.Of<BookSearchRequest>();
            bookSearchRequest.Query = "test";
            bookSearchRequest.StartIndex = 1;
            bookSearchRequest.PageSize = 10;

            var bookSearchResponseObj = SetupTest(true).GetGoogleBooks(bookSearchRequest).Result;
            var bookSearchResponse = (BookSearchResponse)bookSearchResponseObj;

            Assert.IsTrue(bookSearchResponse.IsSuccess);
            Assert.AreEqual(bookSearchResponse.Result.totalItems, 100);
        }

        [Test]
        public void GetGoogleBooksFailTest()
        {
            var bookSearchRequest = Mock.Of<BookSearchRequest>();
            bookSearchRequest.Query = "test";
            bookSearchRequest.StartIndex = 1;
            bookSearchRequest.PageSize = 10;

            try
            {
                var bookSearchResponseObj = SetupTest(false).GetGoogleBooks(bookSearchRequest).Result;
            }
            catch(Exception ex)
            {
                Assert.AreEqual(ex.Message, "One or more errors occurred. (Response status code does not indicate success: 400 (Bad Request).)");
            }
        }

        private GoogleApiManager SetupTest(bool isSuccess)
        {
            var bookSearchHistoryRepositoryMock = new Mock<IBookSearchHistoryRepository>();

            var loggerMock = Mock.Of<ILogger<IGoogleApiManager>>();
            var googleBooksAPIMock = Mock.Of<IOptions<GoogleBooksAPI>>();

            var bookSearchRequest = Mock.Of<BookSearchRequest>();
            bookSearchRequest.Query = "test";
            bookSearchRequest.StartIndex = 1;
            bookSearchRequest.PageSize = 10;
            
            var item = new Item("test", "test", "etag", "selfLink", null, null, null, null);
            var itemList = new List<Item>();
            itemList.Add(item);

            var fakeBaseAddress = "https://www.example.com";

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpResponseMessage = new Mock<HttpResponseMessage>();
            httpMessageHandler.SetupSendAsync(HttpMethod.Get, $"{fakeBaseAddress}/volumes?q={bookSearchRequest.Query}&startIndex={bookSearchRequest.StartIndex}&maxResults={bookSearchRequest.PageSize}")
                                                                                    .ReturnsHttpResponseAsync(new Root("test", 100, itemList), isSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            
            var httpClient = new HttpClient(httpMessageHandler.Object)
            {
                BaseAddress = new Uri(fakeBaseAddress)
            };

            httpClient.DefaultRequestHeaders.Add("api-key", "testapi");

            bookSearchHistoryRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<BookSearchHistory>()));

            var googleApiManager = new GoogleApiManager(loggerMock, bookSearchHistoryRepositoryMock.Object, googleBooksAPIMock, httpClient);
            
            return googleApiManager;

        }
    }
}