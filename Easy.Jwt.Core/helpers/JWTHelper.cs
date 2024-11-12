using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// JwtToken生成、校验（RSA）
    /// </summary>
    public static class JWTHelper
    {
        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="claims"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static string GetJwtToken(SigningCredentials credentials, IEnumerable<Claim> claims, string issuer, string audience, int expire, out DateTime expiresAt)
        {
            var now = DateTime.UtcNow;
            expiresAt = now.AddSeconds(expire);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 读取令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static JwtSecurityToken ReadJwtToken(string token)
        {
            try
            {
                return new JwtSecurityTokenHandler().ReadJwtToken(token);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据令牌获取Claims信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> GetClaims(string token)
        {
            return ReadJwtToken(token)?.Claims;
        }

        public static string GetClaimValue(this IEnumerable<Claim> claims, string key, StringComparison ignoreCase = StringComparison.OrdinalIgnoreCase)
        {
            return claims?.FirstOrDefault(s => s.Type.Equals(key, ignoreCase))?.Value;
        }

        /// <summary>
        /// 获取令牌过期时间
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DateTime? GetExpAt(string token)
        {
            var exp = GetClaims(token)?.GetClaimValue("exp");

            if (exp != null && long.TryParse(exp, out long timestamp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
            }

            return null;
        }

        public static DateTime? GetExpAt(IEnumerable<Claim> claims)
        {
            var exp = claims?.GetClaimValue("exp");

            if (exp != null && long.TryParse(exp, out long timestamp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
            }

            return null;
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static JwtValidateResult VerifyJwtToken(string token, SecurityKey key, string issuer, string audience)
        {
            var result = new JwtValidateResult();

            //校验token
            var validateParameter = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = key,

                ClockSkew = TimeSpan.Zero//校验过期时间必须加此属性
            };

            try
            {
                //校验并解析token,validatedToken是解密后的对象
                result.Principal = new JwtSecurityTokenHandler().ValidateToken(token, validateParameter, out SecurityToken validatedToken);
                var exp = result.Principal.Claims?.GetClaimValue("exp");
                if (exp != null && long.TryParse(exp, out long timestamp))
                {
                    result.ExpAt = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
                }

                result.Success = true;
            }
            catch (SecurityTokenExpiredException)
            {
                result.ErrorType = JwtValidateError.ExpiredError;
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                result.ErrorType = JwtValidateError.AudienceError;
            }
            catch (SecurityTokenException)
            {
                result.ErrorType = JwtValidateError.TokenError;
            }
            catch
            {
                result.ErrorType = JwtValidateError.UnKnownError;
            }

            return result;
        }
    }

    public class JwtValidateResult
    {
        public ClaimsPrincipal Principal { get; set; }

        public bool Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpAt { get; set; }
        
        public JwtValidateError? ErrorType { get; set; }
    }
}