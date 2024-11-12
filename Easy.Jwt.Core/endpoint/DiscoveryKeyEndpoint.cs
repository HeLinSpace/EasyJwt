using h.general.extensions;
using h.general.tools;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    internal class DiscoveryKeyEndpoint : IEndpointHandler
    {
        protected readonly JwtSettings _jwtSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtSettings"></param>
        public DiscoveryKeyEndpoint(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        string IEndpointHandler.Method => "Get";

        string IEndpointHandler.Path => "/.well-known/openid-configuration/jwks";

        string IEndpointHandler.EndpointKey => "openid-configuration-jwks";

        public async Task ProcessAsync(HttpContext context)
        {
            var webKeys = new List<JsonWebKey>();

            if (_jwtSettings.SigningCredentials.Key is X509SecurityKey x509Key)
            {
                var cert64 = Convert.ToBase64String(x509Key.Certificate.RawData);
                var thumbprint = Base64Url.Encode(x509Key.Certificate.GetCertHash());

                if (x509Key.PublicKey is RSA rsa)
                {
                    var parameters = rsa.ExportParameters(false);
                    var exponent = Base64Url.Encode(parameters.Exponent);
                    var modulus = Base64Url.Encode(parameters.Modulus);

                    var rsaJsonWebKey = new JsonWebKey
                    {
                        Kty = "RSA",
                        Use = "sig",
                        Kid = x509Key.KeyId,
                        X5t = thumbprint,
                        E = exponent,
                        N = modulus,
                        X5c = new[] { cert64 },
                        Alg = _jwtSettings.SigningCredentials.Algorithm
                    };
                    webKeys.Add(rsaJsonWebKey);
                }
                else if (x509Key.PublicKey is ECDsa ecdsa)
                {
                    var parameters = ecdsa.ExportParameters(false);
                    var x = Base64Url.Encode(parameters.Q.X);
                    var y = Base64Url.Encode(parameters.Q.Y);

                    var ecdsaJsonWebKey = new JsonWebKey
                    {
                        Kty = "EC",
                        Use = "sig",
                        Kid = x509Key.KeyId,
                        X5t = thumbprint,
                        X = x,
                        Y = y,
                        Crv = CommonHelper.GetCrvValueFromCurve(parameters.Curve),
                        X5c = new[] { cert64 },
                        Alg = _jwtSettings.SigningCredentials.Algorithm
                    };
                    webKeys.Add(ecdsaJsonWebKey);
                }
                else
                {
                    throw new InvalidOperationException($"key type: {x509Key.PublicKey.GetType().Name} not supported.");
                }
            }
            else if (_jwtSettings.SigningCredentials.Key is RsaSecurityKey rsaKey)
            {
                var parameters = rsaKey.Rsa?.ExportParameters(false) ?? rsaKey.Parameters;
                var exponent = Base64Url.Encode(parameters.Exponent);
                var modulus = Base64Url.Encode(parameters.Modulus);

                var webKey = new JsonWebKey
                {
                    Kty = "RSA",
                    Use = "sig",
                    Kid = rsaKey.KeyId,
                    E = exponent,
                    N = modulus,
                    Alg = _jwtSettings.SigningCredentials.Algorithm
                };

                webKeys.Add(webKey);
            }
            else if (_jwtSettings.SigningCredentials.Key is ECDsaSecurityKey ecdsaKey)
            {
                var parameters = ecdsaKey.ECDsa.ExportParameters(false);
                var x = Base64Url.Encode(parameters.Q.X);
                var y = Base64Url.Encode(parameters.Q.Y);

                var ecdsaJsonWebKey = new JsonWebKey
                {
                    Kty = "EC",
                    Use = "sig",
                    Kid = ecdsaKey.KeyId,
                    X = x,
                    Y = y,
                    Crv = CommonHelper.GetCrvValueFromCurve(parameters.Curve),
                    Alg = _jwtSettings.SigningCredentials.Algorithm
                };
                webKeys.Add(ecdsaJsonWebKey);
            }
            else if (_jwtSettings.SigningCredentials.Key is Microsoft.IdentityModel.Tokens.JsonWebKey jsonWebKey)
            {
                var webKey = new JsonWebKey
                {
                    Kty = jsonWebKey.Kty,
                    Use = jsonWebKey.Use ?? "sig",
                    Kid = jsonWebKey.Kid,
                    X5t = jsonWebKey.X5t,
                    E = jsonWebKey.E,
                    N = jsonWebKey.N,
                    X5c = jsonWebKey.X5c?.Count == 0 ? null : jsonWebKey.X5c.ToArray(),
                    Alg = jsonWebKey.Alg,
                    Crv = jsonWebKey.Crv,
                    X = jsonWebKey.X,
                    Y = jsonWebKey.Y
                };

                webKeys.Add(webKey);
            }

            var keys = new Dictionary<string, object>
            {
                { "keys", webKeys },
            };

            var json = ObjectSerializer.Serialize(keys);

            await context.Response.WriteJsonAsync(json);
        }
    }
}
