using Books.API.Constants;
using System.ComponentModel.DataAnnotations;

namespace Books.API.Models.DTOs
{
    public class UserDto
    {
        [MinLength(AppConstants.UserIdMinimumLength)]
        public string UserId { get; set; }
        [MinLength(AppConstants.SessionIdMinimumLength)]
        public string SessionId { get; set; }
    }
}
