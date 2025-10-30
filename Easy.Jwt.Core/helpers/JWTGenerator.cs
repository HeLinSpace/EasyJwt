using Easy.Jwt.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// Jwt 令牌生成、校验（RSA）
    /// </summary>
    internal class JWTGenerator : ITokenGenerator
    {
        protected readonly JwtSettings _jwtSettings;

        protected readonly string[] _defaultClaims = new string[] { JwtClaimTypes.AuthenticationTime, JwtClaimTypes.Expiration, JwtClaimTypes.NotBefore, JwtClaimTypes.Issuer, JwtClaimTypes.Audience, JwtClaimTypes.JwtId };

        public JWTGenerator(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="private_key"></param>
        /// <param name="claims"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public TokenInfo GenerateToken(IEnumerable<Claim> customClaims, JwtClient client)
        {
            var result = new TokenInfo { IsSuccess = false };

            var claims = new List<Claim>
            {
                new(JwtClaimTypes.JwtId, CommonHelper.NewGuid),
                new(JwtClaimTypes.ClientId, client.ClientId),
                new(JwtClaimTypes.AuthenticationTime, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            };

            var scopes = new List<string> { JwtConsts.LocalApi.ScopeName };

            if (client.Scopes.IsPresent())
            {
                scopes.AddRange(client.Scopes);
            }

            foreach (var item in scopes)
            {
                claims.Add(new(JwtClaimTypes.Scope, item));
            }

            if (_jwtSettings.DefaultClaims.IsPresent())
            {
                claims.AddRange(_jwtSettings.DefaultClaims);
            }

            var customClaimsWithoutDefault = customClaims.Where(s => !_defaultClaims.Contains(s.Type)).ToList();

            if (customClaims.IsPresent())
            {
                claims.AddRange(customClaimsWithoutDefault);
            }

            var expires = client.Expires ?? _jwtSettings.Expires;

            result.AccessToken = JWTHelper.GetJwtToken(_jwtSettings.SigningCredentials, claims, _jwtSettings.Issuer, client.Audience, expires, out var expiresAt);

            result.ExpiresAt = expiresAt;
            result.ExpiresIn = expires;
            result.Scope = string.Join(" ", scopes);
            result.TokenType = _jwtSettings.TokenType;
            result.IsSuccess = true;

            return result;
        }

        /// <summary>
        /// 解析令牌信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Claim> GetClaims(string token)
        {
            return JWTHelper.GetClaims(token);
        }

        /// <summary>
        /// 获取过期时间
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public DateTime? GetExpAt(string token)
        {
            return JWTHelper.GetExpAt(token);
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token"></param>
        /// <param name="publicKey">使用RSA私钥创建令牌时必须</param>
        /// <returns></returns>
        public JwtValidateResult VerifyJwtToken(string token, JwtClient client)
        {
            return JWTHelper.VerifyJwtToken(token, _jwtSettings.SigningCredentials.Key, _jwtSettings.Issuer, client.Audience);
        }
    }
}