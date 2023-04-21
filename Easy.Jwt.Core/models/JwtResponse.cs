using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{

    public class JwtResponse
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

        [JsonExtensionData]
        public Dictionary<string, object> Custom { get; set; }

        private static JsonSerializerOptions _default = new JsonSerializerOptions()
        {
#if NETCOREAPP3_1
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
#endif
#if NET5_0_OR_GREATER
            IgnoreReadOnlyFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
#endif
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task ExecuteAsync(HttpContext context)
        {
            var json = JsonSerializer.Serialize(this, _default);
            await context.Response.WriteJsonAsync(json);
        }
    }

}
