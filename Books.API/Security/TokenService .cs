﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Books.API.Models.DTOs;
using Books.API.Extensions;
using Books.API.Constants;

namespace Books.API.Security
{
    public class TokenService : ITokenService
    {
        private readonly OktaJwtVerificationOptions _oktaJwtVerificationOptions;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly ILogger<ITokenService> logger;
        private OktaToken _token = new ();

        public TokenService(ILogger<ITokenService> logger, IOptions<OktaJwtVerificationOptions> oktaJwtVerificationOptions)
        {
            _oktaJwtVerificationOptions = oktaJwtVerificationOptions.Value;
            
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                _oktaJwtVerificationOptions.Issuer + "/.well-known/oauth-authorization-server",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            this.logger = logger;
        }

        public async Task<OktaToken?> GetToken(string userId, string sessionId, string userName, string password)
        {
            var userInfo = new UserDto
            {
                UserId = userId,
                SessionId = sessionId
            };

            if (_token.IsValidAndNotExpiring)
            {
                return _token;
            }

            return await GetNewAccessToken(userInfo, userName: userName, password: password);            
        }

        public async Task<OktaToken?> GetTokenwithRefreshToken(string userId, string sessionId, string refreshToken)
        {
            var userInfo = new UserDto
            {
                UserId = userId,
                SessionId = sessionId
            };            

            return await GetNewAccessToken(userInfo, refreshToken: refreshToken);
        }

        public async Task<JwtSecurityToken?> ValidateToken(string token, UserDto userInfo, CancellationToken ct = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (string.IsNullOrEmpty(_oktaJwtVerificationOptions.Issuer))
            {
                throw new ArgumentNullException(nameof(_oktaJwtVerificationOptions.Issuer));
            }

            var discoveryDocument = await _configurationManager.GetConfigurationAsync(ct);
            var signingKeys = discoveryDocument.SigningKeys;

            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuer = true,
                ValidIssuer = _oktaJwtVerificationOptions.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2),
                ValidateAudience = false,
            };

            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, validationParameters, out var rawValidatedToken);

                return (JwtSecurityToken)rawValidatedToken;
            }
            catch (Exception ex)
            {
                // Logging the exception                
                logger.LogAppError(userInfo, ex);
                return null;
            }
        }

        private async Task<OktaToken?> GetNewAccessToken(UserDto userInfo, string userName = "", string password =  "", string refreshToken = "")
        {
            var client = new HttpClient();
            var clientId = _oktaJwtVerificationOptions.ClientId;
            var clientSecret = _oktaJwtVerificationOptions.ClientSecret;
            var clientCreds = System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(clientCreds));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var postMessage = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(refreshToken))
            {
                postMessage.Add("grant_type", "password");
                postMessage.Add("username", userName);
                postMessage.Add("password", password);
                postMessage.Add("scope", "offline_access openid");                
            }
            else
            {
                postMessage.Add("grant_type", "refresh_token");
                postMessage.Add("scope", "offline_access openid");
                postMessage.Add("refresh_token", refreshToken);                
            }
            
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_oktaJwtVerificationOptions.Issuer}/v1/token")
            {
                Content = new FormUrlEncodedContent(postMessage)                
            };
                        
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var newToken = JsonConvert.DeserializeObject<OktaToken>(json);
                if (newToken != null)
                {
                    newToken.ExpiresAt = DateTime.UtcNow.AddSeconds(newToken.ExpiresIn);
                    return newToken;
                }                
            }
            
            // Logging the exception                
            logger.LogAppError(userInfo, errorMessage : AppConstants.UnauthorizedErrorMessage);
            return null;
        }
    }
}
