using Books.API.Controllers;
using Books.API.Security;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Books.Test.Controllers
{
    [TestFixture]
    public class AuthenticatorControllerTest
    {
        public const int SuccessStatusCode = 200;
        public const int FailStatusCode = 400;

        [Test]
        public void AuthenticateSuccessTest()
        {
            var apiCredentials = new ApiCredentials();
            apiCredentials.UserName = "test@test.com";
            apiCredentials.Password = "testpass1234!@#$";
            
            var booksResponse = SetupTest(true).Authenticate(apiCredentials, "12356", "abcd7").Result;
            var okObjectResult = (OkObjectResult)booksResponse;

            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(okObjectResult.StatusCode.Value, SuccessStatusCode);
        }

        [Test]
        public void AuthenticateFailTest()
        {
            var apiCredentials = new ApiCredentials();
            apiCredentials.UserName = "test@test.com";
            apiCredentials.Password = "testpass1234#$#$";

            var booksResponse = SetupTest(false).Authenticate(apiCredentials, "12356", "abcd5").Result;
            var badRequestResult = (BadRequestObjectResult)booksResponse;

            Assert.IsNotNull(badRequestResult.Value);
            Assert.AreEqual(badRequestResult.StatusCode.Value, FailStatusCode);
        }

        private AuthenticatorController SetupTest(bool isSuccess)
        {
            var tokenServiceMock = new Mock<ITokenService>();

            var oktaToken = new OktaToken() { AccessToken = "eyJraWQiOiJuSEsybUJqRWxVMm9DMDJXYXM3VG8xQWRTUk", TokenType = "Bearer", Scope = "openid" };

            tokenServiceMock.Setup(x => x.GetToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()).Result)
                                .Returns(isSuccess ? oktaToken : null);

            var authenticatorController = new AuthenticatorController(tokenServiceMock.Object);

            return authenticatorController;

        }
    }
}