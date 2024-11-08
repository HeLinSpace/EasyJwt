using System;
using System.Text.Json.Serialization;

namespace Easy.Jwt.Core
{
    public class TokenInfo
    {
        /// <summary>
        /// token
        /// </summary>
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        [JsonPropertyName("expiresAt")]
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// token类型
        /// </summary>
        [JsonPropertyName("tokenType")]
        public string TokenType { get; set; }

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
