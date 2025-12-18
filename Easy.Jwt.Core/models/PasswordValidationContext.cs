using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// Class describing the resource owner password validation context
    /// </summary>
    public class PasswordValidationContext
    {
        /// <summary>
        /// 客户端id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端密码
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// form data
        /// </summary>
        public NameValueCollection FormCollection { get; internal set; }

        /// <summary>
        /// request headers
        /// </summary>
        public NameValueCollection Headers { get; internal set; }

        /// <summary>
        /// Custom fields witch will be  included in the token.
        /// </summary>
        public List<Claim> CustomClaims { get; set; }

        /// <summary>
        /// Custom fields for the token response
        /// </summary>
        public Dictionary<string, object> CustomResponse { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 当前认证客户端
        /// </summary>
        public JwtClient Client { get; internal set; }
    }
}