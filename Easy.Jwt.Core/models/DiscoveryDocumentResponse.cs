using Easy.Jwt.Core.Extensions;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    public class DiscoveryDocumentResponse
    {
        public Exception Exception { get; internal set; }

        public bool IsError => ErrorMessage.IsPresent();

        public string ErrorMessage { get; internal set; }

        public HttpResponseMessage HttpResponse { get; internal set; }

        public string Raw { get; protected set; }

        public JsonElement Json { get; protected set; }

        public JsonWebKeySet KeySet { get; set; }

        public string Issuer => TryGetString(JwtConsts.Discovery.Issuer);
        public string AuthorizeEndpoint => TryGetString(JwtConsts.Discovery.AuthorizationEndpoint);
        public string TokenEndpoint => TryGetString(JwtConsts.Discovery.TokenEndpoint);
        public string UserInfoEndpoint => TryGetString(JwtConsts.Discovery.UserInfoEndpoint);
        public string JwksUri => TryGetString(JwtConsts.Discovery.JwksUri);
        public string EndSessionEndpoint => TryGetString(JwtConsts.Discovery.EndSessionEndpoint);
        public string CheckSessionIframe => TryGetString(JwtConsts.Discovery.CheckSessionIframe);

        internal string TryGetString(string name) => TryGetString(Json, name);

        /// <summary>
        /// Tries to get a string from a JObject
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        internal static string TryGetString(JsonElement? json, string name)
        {
            if (json != null)
            {
                if (json.Value.ValueKind == JsonValueKind.Undefined)
                {
                    return default;
                }

                var res = json.Value.TryGetProperty(name, out JsonElement value) ? value : default;
                return res.ValueKind == JsonValueKind.Undefined ? null : res.ToString();
            }
            return null;
        }

        internal static async Task<DiscoveryDocumentResponse> FromHttpResponseAsync<T>(HttpResponseMessage httpResponse, bool skipJson = false)
        {
            var response = new DiscoveryDocumentResponse
            {
                HttpResponse = httpResponse
            };

            // try to read content
            var content = string.Empty;
            try
            {
                // In .NET, empty content is represented in an HttpResponse with the EmptyContent type,
                // the Content property is not nullable, and ReadAsStringAsync returns the empty string.
                //
                // BUT, in .NET Framework, empty content is represented with a null, and attempting to
                // call ReadAsStringAsync would throw a NRE.
                if (httpResponse.Content != null)
                {
                    content = await httpResponse.Content.ReadAsStringAsync();
                }
                response.Raw = content;
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            // some HTTP error - try to parse body as JSON but allow non-JSON as well
            if (httpResponse.IsSuccessStatusCode != true &&
                httpResponse.StatusCode != HttpStatusCode.BadRequest)
            {
                if (!skipJson && content.IsPresent())
                {
                    try
                    {
                        response.Json = JsonDocument.Parse(content!).RootElement;
                    }
                    catch { }
                }

                return response;
            }

            // either 200 or 400 - both cases need a JSON response (if present), otherwise error
            try
            {
                if (!skipJson && content.IsPresent())
                {
                    response.Json = JsonDocument.Parse(content!).RootElement;
                }
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }
    }
}
