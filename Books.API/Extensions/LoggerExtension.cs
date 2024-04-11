using Books.API.Models.DTOs;

namespace Books.API.Extensions
{
    public static class LoggerExtension
    {
        /// <summary>
        /// Log the error messages in the configured log file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="userInfo"></param>
        /// <param name="ex"></param>
        /// <param name="errorMessage"></param>
        public static void LogAppError<T>(this ILogger<T> logger, UserDto userInfo, Exception? ex = null, string errorMessage = "")
        {
            if(ex != null)
                logger.LogError(ex, $"{Environment.NewLine}UserId: '{userInfo.UserId}' {Environment.NewLine}SessionId: '{userInfo.SessionId}'");
            else
                logger.LogError($"{Environment.NewLine}UserId: '{userInfo.UserId}' {Environment.NewLine}SessionId: '{userInfo.SessionId}'{Environment.NewLine}{errorMessage}");
        }

        /// <summary>
        /// Log the info messages in the configured log file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="userInfo"></param>
        public static void LogAppInfo<T>(this ILogger<T> logger, string message, UserDto userInfo)
        {
            logger.LogInformation($"{Environment.NewLine}UserId: '{userInfo.UserId}' {Environment.NewLine}SessionId: '{userInfo.SessionId}'{Environment.NewLine}{message}");
        }
    }
}
