using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.ExceptionManager
{
    public class ExceptionManager : IExceptionHandler
    {
        private readonly ILogger<ExceptionManager> logger;

        public ExceptionManager(ILogger<ExceptionManager> logger)
        {
            this.logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, $"Exception Occured {exception.Message}");

            var problemDetail = new ValidationProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unknown Error Occured",
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetail, cancellationToken);

            return true;
        }
    }
}
