using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Books.API.Models.DTOs;
using Books.API.Extensions;
using Books.API.Constants;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Security.OktaTokenService
{
    public class TokenService : ITokenService
    {
        private readonly OktaJwtVerificationOptions _oktaJwtVerificationOptions;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly ILogger<ITokenService> logger;
        private OktaToken _token = new();
        private readonly IDistributedCache _cache;

        public TokenService(ILogger<ITokenService> logger, IOptions<OktaJwtVerificationOptions> oktaJwtVerificationOptions, IDistributedCache cache)
        {
            _oktaJwtVerificationOptions = oktaJwtVerificationOptions.Value;
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                _oktaJwtVerificationOptions.Issuer + "/.well-known/oauth-authorization-server",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());
            _cache = cache;
            this.logger = logger;
        }

        /// <summary>
        /// Get the acccess and refresh token from Okta 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<OktaToken?> GetToken(string userId, string sessionId, string userName, string password)
        {
            var userInfo = new UserDto
            {
                UserId = userId,
                SessionId = sessionId
            };

            var _token = await GetTokensFromCache(userInfo);
            if (_token != null)
                return _token;           

            return await GetNewAccessToken(userInfo, userName: userName, password: password);
        }

        /// <summary>
        /// Get a new access token and refresh token using the existing refresh token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<OktaToken?> GetTokenwithRefreshToken(string userId, string sessionId, string refreshToken)
        {
            var userInfo = new UserDto
            {
                UserId = userId,
                SessionId = sessionId
            };
            
            return await GetNewAccessToken(userInfo, refreshToken: refreshToken);
        }

        /// <summary>
        /// Validate the access token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userInfo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<JwtSecurityToken?> ValidateToken(string token, UserDto userInfo, CancellationToken ct = default)
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

        /// <summary>
        /// Revoke the access and refresh token for the provide refresh token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task RevokeRefreshAndAccessToken(string userId, string sessionId, string refreshToken)
        {
            var userInfo = new UserDto
            {
                UserId = userId,
                SessionId = sessionId
            };
            
            await RevokeTokens(userInfo, refreshToken);
        }

        /// <summary>
        /// Get the acccess and refresh token from Okta and add the tokens to the cache
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private async Task<OktaToken?> GetNewAccessToken(UserDto userInfo, string userName = "", string password = "", string refreshToken = "")
        {
            var client = new HttpClient();
            var clientId = _oktaJwtVerificationOptions.ClientId;
            var clientSecret = _oktaJwtVerificationOptions.ClientSecret;
            var clientCreds = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");

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
                    await AddTokensToCache(userInfo, newToken);
                    return newToken;
                }
            }

            // Logging the error                
            logger.LogAppError(userInfo, errorMessage: AppConstants.UnauthorizedErrorMessage);
            return null;
        }

        /// <summary>
        /// Revoke the access and refresh token for the provide refresh token
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private async Task RevokeTokens(UserDto userInfo, string refreshToken)
        {
            var client = new HttpClient();
            var clientId = _oktaJwtVerificationOptions.ClientId;
            var clientSecret = _oktaJwtVerificationOptions.ClientSecret;
            var clientCreds = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(clientCreds));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var postMessage = new Dictionary<string, string>();

            postMessage.Add("token", refreshToken);
            postMessage.Add("token_type_hint", "refresh_token");
            
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_oktaJwtVerificationOptions.Issuer}/v1/revoke")
            {
                Content = new FormUrlEncodedContent(postMessage)
            };

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                logger.LogAppInfo(AppConstants.revokedTokensSuccessful, userInfo);

            // Logging the error
            logger.LogAppInfo(AppConstants.revokedTokensFail, userInfo);            
        }

        /// <summary>
        /// Get tokens from cache
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private async Task<OktaToken?> GetTokensFromCache(UserDto userInfo)
        {
            string cacheKeyAccessToken = string.Concat(userInfo.UserId, userInfo.SessionId, "accesstoken");
            string cacheKeyRefereshToken = string.Concat(userInfo.UserId, userInfo.SessionId, "refreshtoken");
            var cacheAccessToken = await _cache.GetAsync(cacheKeyAccessToken);
            var cacheRefereshToken = await _cache.GetAsync(cacheKeyRefereshToken);

            if (cacheAccessToken != null && cacheRefereshToken != null)
            {
                _token.AccessToken = Encoding.UTF8.GetString(cacheAccessToken);
                _token.RefreshToken = Encoding.UTF8.GetString(cacheRefereshToken);
                logger.LogAppInfo(AppConstants.tokenFoundInCache, userInfo);
                return _token;
            }
            logger.LogAppInfo(AppConstants.tokenNotFoundInCache, userInfo);
            return null;
        }

        /// <summary>
        /// Remove the old tokens and add the new tokens to cache
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="newToken"></param>
        /// <returns></returns>
        private async Task AddTokensToCache(UserDto userInfo, OktaToken newToken)
        {
            string cacheKeyAccessToken = string.Concat(userInfo.UserId, userInfo.SessionId, "accesstoken");
            string cacheKeyRefereshToken = string.Concat(userInfo.UserId, userInfo.SessionId, "refreshtoken");
            var cacheAccessToken = await _cache.GetAsync(cacheKeyAccessToken);
            var cacheRefereshToken = await _cache.GetAsync(cacheKeyRefereshToken);

            //remove the old tokens
            if (cacheAccessToken != null && cacheRefereshToken != null)
            {
                await _cache.RemoveAsync(cacheKeyAccessToken);
                await _cache.RemoveAsync(cacheKeyRefereshToken);
                logger.LogAppInfo(AppConstants.tokenRemovedInCache, userInfo);
            }

            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(_oktaJwtVerificationOptions.ExpiresInSeconds));
            byte[] bytesAccessToken = Encoding.ASCII.GetBytes(newToken.AccessToken);
            byte[] bytesRefreshToken = Encoding.ASCII.GetBytes(newToken.RefreshToken);
            await _cache.SetAsync(cacheKeyAccessToken, bytesAccessToken, options);
            await _cache.SetAsync(cacheKeyRefereshToken, bytesRefreshToken, options);
            logger.LogAppInfo(AppConstants.tokenAddedInCache, userInfo);
        }
    }
}
