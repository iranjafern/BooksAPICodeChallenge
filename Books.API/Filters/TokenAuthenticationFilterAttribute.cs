using Books.API.Constants;
using Books.API.Models.DTOs;
using Books.API.Security.OktaTokenService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Collections.Specialized;
using System.Web;

namespace Books.API.Filters
{
    public class TokenAuthenticationFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private const string UserIdSessionIdError = "Missing userId and(/or) sessionId";
        private readonly ITokenService tokenService;

        public TokenAuthenticationFilterAttribute(ITokenService tokenService)
        {
            this.tokenService = tokenService;            
        }

        /// <summary>
        /// Authorize the requests made to the GetBooks endpoint
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AppConstants.Authorization, out var token))
            {
                context.Result = GenerateUnauthorizedResult(context);
                return;
            }

            var userInfo = GetUserFromQueryString(HttpUtility.ParseQueryString(context.HttpContext.Request.QueryString.Value));

            if(userInfo == null)
            {
                context.Result = GenerateBadRequestResult(context);
                return;
            }


            var validatedToken = await tokenService.ValidateToken(token.ToString().Split(" ")[1], userInfo);

            if (validatedToken == null)
            {
                context.Result = GenerateUnauthorizedResult(context);
            }
        }

        private IActionResult GenerateUnauthorizedResult(AuthorizationFilterContext context)
        {
            context.ModelState.AddModelError(AppConstants.UnauthorizedErrorKey, AppConstants.UnauthorizedErrorMessage);
            var problemDetail = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status401Unauthorized
            };
            return new BadRequestObjectResult(problemDetail);
        }

        private IActionResult GenerateBadRequestResult(AuthorizationFilterContext context)
        {
            context.ModelState.AddModelError(AppConstants.BadRequestErrorKey, UserIdSessionIdError);
            var problemDetail = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problemDetail);
        }

        private UserDto GetUserFromQueryString(NameValueCollection queryString)
        {
            UserDto user = null;
            if (queryString != null)
            {
                var userId = queryString["userId"];
                var sessionId = queryString["sessionId"];

                if(!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(sessionId))
                {
                    user = new UserDto
                    {
                        UserId = userId,
                        SessionId = sessionId
                    };
                }
            }

            return user;
        }
    }
}
