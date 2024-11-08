using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    public class JwtResponse: TokenInfo
    {
        public JwtResponse() { }

        public JwtResponse(TokenInfo tokenInfo) 
        {
            AccessToken = tokenInfo.AccessToken;
            ExpiresAt = tokenInfo.ExpiresAt;
            TokenType = tokenInfo.TokenType;
            Error = tokenInfo.Error;
            IsSuccess = tokenInfo.IsSuccess;
        }

        [JsonExtensionData]
        public Dictionary<string, object> Custom { get; set; }

        private static JsonSerializerOptions _default = new()
        {
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task ExecuteAsync(HttpContext context)
        {
            var json = JsonSerializer.Serialize(this, _default);

            await context.Response.WriteJsonAsync(json);
        }
    }
}
