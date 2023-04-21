using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.IO;

#pragma warning disable 1591

namespace Easy.Jwt.Core
{
    public static class HttpRequestExtensions
    {
        private static JsonSerializerOptions _default = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        internal static bool HasJsonContentType(this HttpRequest request)
        {
            return request.HasJsonContentType(out _);
        }

        internal static bool HasJsonContentType(this HttpRequest request, out StringSegment charset)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!MediaTypeHeaderValue.TryParse(request.ContentType, out var mt))
            {
                charset = StringSegment.Empty;
                return false;
            }

            // Matches application/json
            if (mt.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
            {
                charset = mt.Charset;
                return true;
            }

            // Matches +json, e.g. application/ld+json
            if (mt.Suffix.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                charset = mt.Charset;
                return true;
            }

            charset = StringSegment.Empty;
            return false;
        }

        public static async Task<TValue> ReadFromJsonAsync<TValue>(this HttpRequest request)
        {
            if (!request.HasJsonContentType(out var charset))
            {
                throw new InvalidOperationException($"Unable to read the request as JSON because the request content type '{request.ContentType}' is not a known JSON content type.");
            }

            var options = ResolveSerializerOptions(request.HttpContext);

            var encoding = GetEncodingFromCharset(charset);
            using var reader = new StreamReader(request.Body, encoding);
            var body = await reader.ReadToEndAsync();

            return  JsonSerializer.Deserialize<TValue>(body, options);
        }

        private static JsonSerializerOptions ResolveSerializerOptions(HttpContext httpContext)
        {
            // Attempt to resolve options from DI then fallback to default options
            return httpContext.RequestServices?.GetService<IOptions<JsonOptions>>()?.Value?.JsonSerializerOptions ?? _default;
        }

        private static Encoding? GetEncodingFromCharset(StringSegment charset)
        {
            if (charset.Equals("utf-8", StringComparison.OrdinalIgnoreCase))
            {
                // This is an optimization for utf-8 that prevents the Substring caused by
                // charset.Value
                return Encoding.UTF8;
            }

            try
            {
                // charset.Value might be an invalid encoding name as in charset=invalid.
                return charset.HasValue ? Encoding.GetEncoding(charset.Value) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unable to read the request as JSON because the request content type charset '{charset}' is not a known encoding.", ex);
            }
        }
    }
}