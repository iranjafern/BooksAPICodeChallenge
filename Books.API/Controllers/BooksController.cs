using Books.API.Constants;
using Books.API.Filters;
using Books.API.GoogleBooksApi;
using Books.API.Models.DTOs;
using Books.API.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;

namespace Books.API.Controllers
{
    [Route("api/internal/[controller]")]
    [ApiController]
    [TypeFilter(typeof(TokenAuthenticationFilterAttribute))]
    public class BooksController : ControllerBase
    {
        private readonly IGoogleApiManager googleApiManager;

        public BooksController(IGoogleApiManager googleApiManager)
        {
            this.googleApiManager = googleApiManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetBooks(string userId, string sessionId, string query, int startIndex, int pageSize = AppConstants.MaxPageSize)
        {
            var response = await googleApiManager.GetBooksAsync(startIndex, pageSize, query, userId, sessionId);
            
            if (response == null || !response.IsSuccess)
            {
                ModelState.AddModelError(AppConstants.BadRequestErrorKey, (response!=null) ? response.Message: AppConstants.BadRequestErrorKey);
                var problemDetail = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };

                return new BadRequestObjectResult(problemDetail);
            }

            return Ok(response);
        }
    }
}
