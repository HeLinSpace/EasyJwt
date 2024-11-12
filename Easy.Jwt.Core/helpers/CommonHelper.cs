using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Easy.Jwt.Core
{

    public static class CommonHelper
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        /// <summary>
        /// 生成新的Guid
        /// </summary>
        /// <returns></returns>
        public static string NewGuid
        {
            get
            {
                return Guid.NewGuid().ToString("N").ToUpper();
            }
        }

        /// <summary>
        /// Creates a new RSA security key.
        /// </summary>
        /// <returns></returns>
        public static RsaSecurityKey CreateRsaSecurityKey(int keySize = 2048)
        {
            return new RsaSecurityKey(RSA.Create(keySize))
            {
                KeyId = CreateUniqueId(16, OutputFormat.Hex)
            };
        }

        /// <summary>
        /// Creates a URL safe unique identifier.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="format">The output format</param>
        /// <returns></returns>
        public static string CreateUniqueId(int length = 32, OutputFormat format = OutputFormat.Base64Url)
        {
            var bytes = CreateRandomKey(length);

            return format switch
            {
                OutputFormat.Base64Url => Base64Url.Encode(bytes),
                OutputFormat.Base64 => Convert.ToBase64String(bytes),
                OutputFormat.Hex => BitConverter.ToString(bytes).Replace("-", ""),
                _ => throw new ArgumentException("Invalid output format", nameof(format)),
            };
        }

        /// <summary>
        /// Creates a random key byte array.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static byte[] CreateRandomKey(int length)
        {
            var bytes = new byte[length];
            Rng.GetBytes(bytes);

            return bytes;
        }

        internal static bool IsValidCurveForAlgorithm(ECDsaSecurityKey key, string algorithm)
        {
            var parameters = key.ECDsa.ExportParameters(false);

            if (algorithm == SecurityAlgorithms.EcdsaSha256 && parameters.Curve.Oid.Value != JwtConsts.CurveOids.P256
                || algorithm == SecurityAlgorithms.EcdsaSha384 && parameters.Curve.Oid.Value != JwtConsts.CurveOids.P384
                || algorithm == SecurityAlgorithms.EcdsaSha512 && parameters.Curve.Oid.Value != JwtConsts.CurveOids.P521)
            {
                return false;
            }

            return true;
        }


        internal static bool IsValidCrvValueForAlgorithm(string crv)
        {
            return crv == JsonWebKeyECTypes.P256 ||
                   crv == JsonWebKeyECTypes.P384 ||
                   crv == JsonWebKeyECTypes.P521;
        }

        internal static X509Certificate2 FindCertificate(string name, StoreLocation location, JwtConsts.NameType nameType)
        {
            X509Certificate2 certificate = null;

            if (location == StoreLocation.LocalMachine)
            {
                if (nameType == JwtConsts.NameType.SubjectDistinguishedName)
                {
                    certificate = X509.LocalMachine.My.SubjectDistinguishedName.Find(name, validOnly: false).FirstOrDefault();
                }
                else if (nameType == JwtConsts.NameType.Thumbprint)
                {
                    certificate = X509.LocalMachine.My.Thumbprint.Find(name, validOnly: false).FirstOrDefault();
                }
            }
            else
            {
                if (nameType == JwtConsts.NameType.SubjectDistinguishedName)
                {
                    certificate = X509.CurrentUser.My.SubjectDistinguishedName.Find(name, validOnly: false).FirstOrDefault();
                }
                else if (nameType == JwtConsts.NameType.Thumbprint)
                {
                    certificate = X509.CurrentUser.My.Thumbprint.Find(name, validOnly: false).FirstOrDefault();
                }
            }

            return certificate;
        }

        internal static string GetRsaSigningAlgorithmValue(JwtConsts.RsaSigningAlgorithm value)
        {
            return value switch
            {
                JwtConsts.RsaSigningAlgorithm.RS256 => SecurityAlgorithms.RsaSha256,
                JwtConsts.RsaSigningAlgorithm.RS384 => SecurityAlgorithms.RsaSha384,
                JwtConsts.RsaSigningAlgorithm.RS512 => SecurityAlgorithms.RsaSha512,

                JwtConsts.RsaSigningAlgorithm.PS256 => SecurityAlgorithms.RsaSsaPssSha256,
                JwtConsts.RsaSigningAlgorithm.PS384 => SecurityAlgorithms.RsaSsaPssSha384,
                JwtConsts.RsaSigningAlgorithm.PS512 => SecurityAlgorithms.RsaSsaPssSha512,
                _ => throw new ArgumentException("Invalid RSA signing algorithm value", nameof(value)),
            };
        }

        internal static string GetECDsaSigningAlgorithmValue(JwtConsts.ECDsaSigningAlgorithm value)
        {
            return value switch
            {
                JwtConsts.ECDsaSigningAlgorithm.ES256 => SecurityAlgorithms.EcdsaSha256,
                JwtConsts.ECDsaSigningAlgorithm.ES384 => SecurityAlgorithms.EcdsaSha384,
                JwtConsts.ECDsaSigningAlgorithm.ES512 => SecurityAlgorithms.EcdsaSha512,
                _ => throw new ArgumentException("Invalid ECDsa signing algorithm value", nameof(value)),
            };
        }

        internal static string GetCrvValueFromCurve(ECCurve curve)
        {
            return curve.Oid.Value switch
            {
                JwtConsts.CurveOids.P256 => JsonWebKeyECTypes.P256,
                JwtConsts.CurveOids.P384 => JsonWebKeyECTypes.P384,
                JwtConsts.CurveOids.P521 => JsonWebKeyECTypes.P521,
                _ => throw new InvalidOperationException($"Unsupported curve type of {curve.Oid.Value} - {curve.Oid.FriendlyName}"),
            };
        }

        [DebuggerStepThrough]
        internal static bool IsValidScheme(Uri url)
        {
            if (string.Equals(url.Scheme, "http", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }


        [DebuggerStepThrough]
        internal static string RemoveTrailingSlash(string url)
        {
            if (url != null && url.EndsWith("/"))
            {
                url = url[..^1];
            }

            return url;
        }


        [DebuggerStepThrough]
        internal static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }
    }
}
