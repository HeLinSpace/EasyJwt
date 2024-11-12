using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Easy.Jwt.Core.Extensions
{
    /// <summary>
    /// HttpClient extentions for OIDC discovery
    /// </summary>
    public static class HttpClientDiscoveryExtensions
    {
        /// <summary>
        /// Sends a discovery document request
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(this HttpClient client, string address = null, CancellationToken cancellationToken = default)
        {
            if (address.IsEmpty())
            {
                address = client.BaseAddress.AbsoluteUri;
            }

            if (address.IsEmpty())
            {
                throw new ArgumentException("An address is required.");
            }

            var parsed = DiscoveryEndpoint.ParseUrl(address);
            var url = parsed.url;

            string jwkUrl = "";

            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(url, cancellationToken);

                string responseContent = null;

                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new DiscoveryDocumentResponse
                    {
                        HttpResponse = response,
                        ErrorMessage = $"Error connecting to {url}: {response.ReasonPhrase}"
                    };
                }

                var disco = await DiscoveryDocumentResponse.FromHttpResponseAsync<DiscoveryDocumentResponse>(response);

                if (disco.IsError)
                {
                    return disco;
                }

                try
                {
                    jwkUrl = disco.JwksUri;
                    if (jwkUrl != null)
                    {
                        var jwkResponse = await client.GetAsync(jwkUrl, cancellationToken);

                        if (jwkResponse.IsSuccessStatusCode)
                        {
                            var jwkStr = jwkResponse.Content.ReadAsStringAsync(cancellationToken).Result;

                            disco.KeySet = new JsonWebKeySet(jwkStr);
                        }
                        else
                        {
                            return new DiscoveryDocumentResponse
                            {
                                HttpResponse = jwkResponse,
                                ErrorMessage = $"Error connecting to {jwkUrl}: {jwkResponse.ReasonPhrase}"
                            };
                        }
                    }

                    return disco;
                }
                catch (Exception ex)
                {
                    return new DiscoveryDocumentResponse
                    {
                        Exception = ex,
                        ErrorMessage = $"Error connecting to {jwkUrl}. {ex.Message}."
                    };
                }
            }
            catch (Exception ex)
            {
                return new DiscoveryDocumentResponse
                {
                    Exception = ex,
                    ErrorMessage = $"Error connecting to {url}. {ex.Message}."
                };
            }
        }

        public static async Task<JwtResponse> GenerateTokenAsync(this HttpClient client, string uri, TokenRequest request, CancellationToken cancellationToken = default)
        {
            string result;

            try
            {
                HttpContent httpContent = new StringContent(ObjectSerializer.Serialize(request));
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(uri, httpContent, cancellationToken);

                result = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                var res = new JwtResponse
                {
                    Error = ex.Message,
                    IsSuccess = false,
                };

                return res;
            }

            return ObjectSerializer.Deserialize<JwtResponse>(result);
        }
    }
}