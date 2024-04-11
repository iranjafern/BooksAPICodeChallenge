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
        public const string tokenFoundInCache = "token found in cache";
        public const string tokenNotFoundInCache = "token not found in cache";
        public const string tokenAddedInCache = "token added to cache";
        public const string tokenRemovedInCache = "token removed from cache";
        public const string revokedTokensSuccessful = "revoked Tokens successfully";
        public const string revokedTokensFail = "revoked Tokens failed";
        public const int MaxPageSize = 40;
        public const int UserIdMinimumLength = 5;
        public const int SessionIdMinimumLength = 5;
    }
}
