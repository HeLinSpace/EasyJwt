using System;
using System.Text.Json.Serialization;

namespace Easy.Jwt.Core
{
    public class TokenInfo
    {
        /// <summary>
        /// token
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        [JsonPropertyName("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// 有效时长
        /// </summary>
        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// token类型
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// scope
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// error
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }

        /// <summary>
        /// 是否生成成功 Token generate success?
        /// </summary>
        [JsonPropertyName("success")]
        public bool IsSuccess { get; set; }
    }
}
