using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// model for client token request
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// 客户端id
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端密码
        /// </summary>
        [JsonPropertyName("client_secret")]
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
        /// Custom fields for the token request
        /// </summary>
        public Dictionary<string, object> CustomProperty { get; set; } = new Dictionary<string, object>();
    }
}