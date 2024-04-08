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
        public async Task<IActionResult> Authenticate([FromBody] ApiCredentials credentials, string userId, string sessionId)
        {            
            var token = await tokenService.GetToken(credentials.UserName, credentials.Password, userId, sessionId);

            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                ModelState.AddModelError(AppConstants.UnauthorizedErrorKey, AppConstants.UnauthorizedErrorMessage);
                var problemDetail = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status401Unauthorized
                };

                return new BadRequestObjectResult(problemDetail);
            }
            else
            {
                return Ok(token);
            }
        }
    }
}
