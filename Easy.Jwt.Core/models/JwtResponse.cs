using h.general.extensions;
using h.general.tools;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
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

        public async Task ExecuteAsync(HttpContext context)
        {
            var json = ObjectSerializer.Serialize(this);

            await context.Response.WriteJsonAsync(json);
        }
    }
}
