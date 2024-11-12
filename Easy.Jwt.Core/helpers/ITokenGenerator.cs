using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// Jwt 令牌生成
    /// </summary>
    public interface ITokenGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="private_key"></param>
        /// <param name="claims"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        TokenInfo GenerateToken(IEnumerable<Claim> claims, JwtClient client);

        /// <summary>
        /// 解析令牌信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<Claim> GetClaims(string token);

        /// <summary>
        /// 获取过期时间
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        DateTime? GetExpAt(string token);

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        JwtValidateResult VerifyJwtToken(string token, JwtClient client);
    }
}