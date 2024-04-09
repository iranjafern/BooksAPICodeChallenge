namespace Books.API.Constants
{
    public class AppConstants
    {
        public const string Authorization = "Authorization";
        public const string UnauthorizedErrorKey = "Unauthorized";
        public const string UnauthorizedErrorMessage = "Application is not authorized to use the API.";
        public const string BadRequestErrorKey = "BadRequest";
        public const string SecurityConfigKey = "Security";
        public const string AllowedOriginsConfigKey = "allowedOrigins";
        public const string InvalidResponse = "Invalid Response";
        public const int MaxPageSize = 40;
        public const int UserIdMinimumLength = 5;
        public const int SessionIdMinimumLength = 5;
    }
}
