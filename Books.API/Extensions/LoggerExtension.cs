using Books.API.Models.DTOs;

namespace Books.API.Extensions
{
    public static class LoggerExtension
    {
        public static void LogAppError<T>(this ILogger<T> logger, Exception ex, UserDto userInfo)
        {
            logger.LogError(ex, $"{Environment.NewLine}UserId: '{userInfo.UserId}' {Environment.NewLine}SessionId: '{userInfo.SessionId}'");
        }

        public static void LogAppInfo<T>(this ILogger<T> logger, string message, UserDto userInfo)
        {
            logger.LogInformation($"{Environment.NewLine}UserId: '{userInfo.UserId}' {Environment.NewLine}SessionId: '{userInfo.SessionId}'{Environment.NewLine}{message}");
        }
    }
}
