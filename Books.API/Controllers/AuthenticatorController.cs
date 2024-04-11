using Books.API.Constants;
using Books.API.Models.Responses;
using Books.API.Security;
using Books.API.Security.OktaTokenService;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers
{
    [Route("api/internal/[controller]")]
    [ApiController]
    public class AuthenticatorController : ControllerBase
    {
        private readonly ITokenService tokenService;        
        public AuthenticatorController(ITokenService tokenService)
        {
            this.tokenService = tokenService;            
        }

        [HttpPost]
        [Route("authenticate")]        
        public async Task<IActionResult> Authenticate([FromBody] ApiCredentials credentials, string userId, string sessionId)
        {   
            //get the acccess and refresh token from Okta
            var token = await tokenService.GetToken(userId, sessionId, credentials.UserName, credentials.Password);
            var badRequestObjectResult = ValidateToken(token);

            //if no valid token returned return Status401Unauthorized error
            if (badRequestObjectResult == null)
            {
                return Ok(token);
            }
            return badRequestObjectResult;
        }

        [HttpPost]
        [Route("authenticatewithrefreshtoken")]
        public async Task<IActionResult> AuthenticatewithRefreshtoken(string userId, string sessionId, string refreshToken)
        {
            //get a new access token and refresh token using the existing refresh token
            var token = await tokenService.GetTokenwithRefreshToken(userId, sessionId, refreshToken);
            var badRequestObjectResult = ValidateToken(token);

            //if no valid token returned return Status401Unauthorized error
            if (badRequestObjectResult == null)
            {
                return Ok(token);
            }
            return badRequestObjectResult;
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout(string userId, string sessionId, string refreshToken)
        {
            //revoke the access and refresh token for the provide refresh token
            await tokenService.RevokeRefreshAndAccessToken(userId, sessionId, refreshToken);
            
            return Ok();
        }

        /// <summary>
        /// If no valid token returned return Status401Unauthorized error
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private BadRequestObjectResult? ValidateToken(OktaToken token)
        {
            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                ModelState.AddModelError(AppConstants.UnauthorizedErrorKey, AppConstants.UnauthorizedErrorMessage);
                var problemDetail = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status401Unauthorized
                };

                return new BadRequestObjectResult(problemDetail);
            }
            return null;
        }
    }
}
