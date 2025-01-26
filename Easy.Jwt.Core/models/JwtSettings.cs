using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Easy.Jwt.Core
{
    public class JwtSettings
    {
        /// <summary>
        /// default in payload
        /// </summary>
        public IEnumerable<Claim> DefaultClaims { get; set; }

        /// <summary>
        /// clients settings
        /// </summary>
        public IEnumerable<JwtClient> Clients { get; set; }

        /// <summary>
        /// clients settings
        /// </summary>
        public Func<IServiceProvider, IEnumerable<JwtClient>> ClientFunc { get; set; }

        /// <summary>
        /// issuer
        /// default *
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// auth server domain
        /// default HttpContext.Request.Host
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// expires on（seconds），default 8 hours
        /// </summary>
        public int Expires { get; set; }

        /// <summary>
        /// token类型 default Bearer
        /// </summary>
        public string TokenType { get; set; }

        private SigningCredentials _signingCredential;

        public SigningCredentials SigningCredentials { get { return _signingCredential; } }

        public void SetSigningCredential(SigningCredentials credential)
        {
            if (!(credential.Key is AsymmetricSecurityKey
                || (credential.Key is Microsoft.IdentityModel.Tokens.JsonWebKey key1 && key1.HasPrivateKey)))
            {
                throw new InvalidOperationException("Signing key is not asymmetric");
            }

            if (!JwtConsts.SupportedSigningAlgorithms.Contains(credential.Algorithm, StringComparer.Ordinal))
            {
                throw new InvalidOperationException($"Signing algorithm {credential.Algorithm} is not supported.");
            }

            if (credential.Key is ECDsaSecurityKey key && !CommonHelper.IsValidCurveForAlgorithm(key, credential.Algorithm))
            {
                throw new InvalidOperationException("Invalid curve for signing algorithm");
            }

            if (credential.Key is Microsoft.IdentityModel.Tokens.JsonWebKey jsonWebKey)
            {
                if (jsonWebKey.Kty == JsonWebAlgorithmsKeyTypes.EllipticCurve && !CommonHelper.IsValidCrvValueForAlgorithm(jsonWebKey.Crv))
                    throw new InvalidOperationException("Invalid crv value for signing algorithm");
            }

            _signingCredential = credential;
        }

        public void SetSigningCredential(X509Certificate2 certificate, string signingAlgorithm = SecurityAlgorithms.RsaSha256)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));

            if (!certificate.HasPrivateKey)
            {
                throw new InvalidOperationException("X509 certificate does not have a private key.");
            }

            var key = new X509SecurityKey(certificate);
            key.KeyId += signingAlgorithm;

            var credential = new SigningCredentials(key, signingAlgorithm);
            SetSigningCredential(credential);
        }

        public void SetSigningCredential(string name, StoreLocation location = StoreLocation.LocalMachine, JwtConsts.NameType nameType = JwtConsts.NameType.SubjectDistinguishedName, string signingAlgorithm = SecurityAlgorithms.RsaSha256)
        {
            var certificate = CommonHelper.FindCertificate(name, location, nameType) ?? throw new InvalidOperationException($"certificate: '{name}' not found in certificate store");
            SetSigningCredential(certificate, signingAlgorithm);
        }

        public void SetSigningCredential(SecurityKey key, string signingAlgorithm)
        {
            var credential = new SigningCredentials(key, signingAlgorithm);
            SetSigningCredential(credential);
        }

        public void SetSigningCredential(RsaSecurityKey key, JwtConsts.RsaSigningAlgorithm signingAlgorithm)
        {
            var credential = new SigningCredentials(key, CommonHelper.GetRsaSigningAlgorithmValue(signingAlgorithm));
            SetSigningCredential(credential);
        }

        public void SetSigningCredential(ECDsaSecurityKey key, JwtConsts.ECDsaSigningAlgorithm signingAlgorithm)
        {
            var credential = new SigningCredentials(key, CommonHelper.GetECDsaSigningAlgorithmValue(signingAlgorithm));
            SetSigningCredential(credential);
        }

        public void SetDeveloperSigningCredential(bool persistKey = true, string filename = null, JwtConsts.RsaSigningAlgorithm signingAlgorithm = JwtConsts.RsaSigningAlgorithm.RS256)
        {
            filename ??= Path.Combine(Directory.GetCurrentDirectory(), "tempkey.jwk");

            if (File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                var jwk = new Microsoft.IdentityModel.Tokens.JsonWebKey(json);

                SetSigningCredential(jwk, jwk.Alg);
            }
            else
            {
                var key = CommonHelper.CreateRsaSecurityKey();
                var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
                jwk.Alg = signingAlgorithm.ToString();

                if (persistKey)
                {
                    File.WriteAllText(filename, ObjectSerializer.Serialize(jwk));
                }

                SetSigningCredential(key, signingAlgorithm);
            }
        }
    }
}
