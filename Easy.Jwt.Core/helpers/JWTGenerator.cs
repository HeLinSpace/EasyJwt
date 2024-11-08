using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// Jwt 令牌生成、校验（RSA）
    /// </summary>
    public class JWTGenerator : ITokenGenerator
    {
        protected readonly JwtSettings _jwtSettings;

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
        public TokenInfo GenerateToken(IEnumerable<Claim> claims)
        {
            var result = new TokenInfo { IsSuccess = false };

            if (_jwtSettings.IssuerSigningKey.IsPresent())
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey));

                result.AccessToken = JWTHelper.GetJwtTokenHS256(key, claims, _jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.Expires, out var expiresAt);
                result.ExpiresAt = expiresAt;
                result.TokenType = _jwtSettings.TokenType;
                result.IsSuccess = true;
            }
            else if (_jwtSettings.PrivateKey.IsPresent())
            {
                result.AccessToken = JWTHelper.GetJwtTokenRS256(_jwtSettings.PrivateKey, claims, _jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.Expires, out var expiresAt);
                result.ExpiresAt = expiresAt;
                result.TokenType = _jwtSettings.TokenType;
                result.IsSuccess = true;
            }
            else
            {
                result.Error = JwtConsts.JwtGenerateError.CredentialError;
            }

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
        public JwtValidateResult VerifyJwtToken(string token, string publicKey = "")
        {
            if (_jwtSettings.IssuerSigningKey.IsPresent())
            {
                return JWTHelper.VerifyJwtToken(token, _jwtSettings.IssuerSigningKey, _jwtSettings.Issuer, _jwtSettings.Audience);
            }
            else if (_jwtSettings.PrivateKey.IsPresent())
            {
                return JWTHelper.VerifyJwtTokenRSA(token, publicKey, _jwtSettings.Issuer, _jwtSettings.Audience);
            }
            else
            {
                return new JwtValidateResult { ErrorType = JwtValidateError.TokenError, Success = false };
            }
        }
    }
}