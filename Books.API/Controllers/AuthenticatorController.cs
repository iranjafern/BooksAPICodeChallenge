using Books.API.Constants;
using Books.API.Models.DTOs;
using Books.API.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
            var token = await tokenService.GetToken(userId, sessionId, credentials.UserName, credentials.Password);
            var badRequestObjectResult = ValidateToken(token);

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
            var token = await tokenService.GetTokenwithRefreshToken(userId, sessionId, refreshToken);
            var badRequestObjectResult = ValidateToken(token);

            if (badRequestObjectResult == null)
            {
                return Ok(token);                
            }
            return badRequestObjectResult;
        }

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
