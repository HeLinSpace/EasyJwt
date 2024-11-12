using Easy.Jwt.Core.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    internal class DiscoveryEndpoint : IEndpointHandler
    {
        protected readonly JwtSettings _jwtSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtSettings"></param>
        public DiscoveryEndpoint(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        string IEndpointHandler.Method => "Get";

        string IEndpointHandler.Path => "/.well-known/openid-configuration";

        string IEndpointHandler.EndpointKey => "openid-configuration";

        public async Task ProcessAsync(HttpContext context)
        {
            var entries = new Dictionary<string, object>
            {
                { "issuer", "http://localhost:22002/" },
                { "jwks_uri", "http://localhost:22002/.well-known/openid-configuration/jwks" },
                { "token_endpoint", "http://localhost:22002/connect/token" },
            };

            var json = ObjectSerializer.Serialize(entries);

            await context.Response.WriteJsonAsync(json);
        }

        public static (string authority, string url) ParseUrl(string input)
        {
            var success = Uri.TryCreate(input, UriKind.Absolute, out var uri);
            if (success == false)
            {
                throw new InvalidOperationException("Malformed URL");
            }

            if (!CommonHelper.IsValidScheme(uri))
            {
                throw new InvalidOperationException("Malformed URL");
            }

            var url = CommonHelper.RemoveTrailingSlash(input);

            if (url.EndsWith(JwtConsts.Discovery.DiscoveryEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                return (url[..(url.Length - JwtConsts.Discovery.DiscoveryEndpoint.Length - 1)], url);
            }
            else
            {
                return (url, CommonHelper.EnsureTrailingSlash(url) + JwtConsts.Discovery.DiscoveryEndpoint);
            }
        }
    }
}
