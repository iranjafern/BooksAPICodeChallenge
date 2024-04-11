using Books.API.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace Books.API.Security.OktaTokenService
{
    public interface ITokenService
    {
        Task<OktaToken?> GetToken(string userName, string password, string userId, string sessionId);
        Task<OktaToken?> GetTokenwithRefreshToken(string userId, string sessionId, string refreshToken);
        Task<JwtSecurityToken?> ValidateToken(string token, UserDto userInfo, CancellationToken ct = default);
    }
}
