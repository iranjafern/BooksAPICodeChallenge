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

        /// <summary>
        /// The global exception handler. All unhandled exceptions will be handler return Status500InternalServerError response
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
