﻿using Books.API.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace Books.API.Security
{
    public interface ITokenService
    {
        Task<OktaToken?> GetToken(string userName, string password, string userId, string sessionId);
        Task<JwtSecurityToken?> ValidateToken(string token, UserDto userInfo, CancellationToken ct = default(CancellationToken));
    }
}