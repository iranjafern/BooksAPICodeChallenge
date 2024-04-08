using Books.API.Controllers;
using Books.API.GoogleBooksApi;
using Books.API.Models.DTOs;
using Books.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Books.Test.Controllers
{
    [TestFixture]
    public class BooksControllerTest
    {
        public const int SuccessStatusCode = 200;
        public const int FailStatusCode = 400;

        [Test]
        public void GetBooksSuccessTest()
        {
            var booksResponse = SetupTest(true).GetBooks("12356","abcd4","test",1,10).Result;
            var okObjectResult = (OkObjectResult)booksResponse;

            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(okObjectResult.StatusCode.Value, SuccessStatusCode);            
        }

        [Test]
        public void GetBooksFailTest()
        {
            var booksResponse = SetupTest(false).GetBooks("12356", "abcd4", "test", 1, 50).Result;
            var badRequestResult = (BadRequestObjectResult) booksResponse;

            Assert.IsNotNull(badRequestResult.Value);
            Assert.AreEqual(badRequestResult.StatusCode.Value, FailStatusCode);
        }

        private BooksController SetupTest(bool isSuccess)
        {
            var googleApiManagerMock = new Mock<IGoogleApiManager>();

            var item = new Item("test", "test", "etag", "selfLink", null, null, null, null);
            var itemList = new List<Item>();
            itemList.Add(item);
            
            var bookSearchResponse = new BookSearchResponse() { IsSuccess = isSuccess, Message = "Test", Result = new Root("test", 3610, itemList) };            
            googleApiManagerMock.Setup(x => x.GetBooksAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()).Result)
                                .Returns(bookSearchResponse);

            var booksController = new BooksController(googleApiManagerMock.Object);

            return booksController;

        }
    }
}