using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace Easy.Jwt.Core
{
    public struct JwtConsts
    {
        internal struct JwtGenerateError
        {
            public const string InitError = "init error: Please call the method serviceCollection.AddEasyJwt() first .";
            public const string PasswordValidatorNotImplementedError = "inject a custom implementation of the IPasswordValidator into the ioc container first .";
            public const string CredentialError = "configuration error: Please call the method serviceCollection.AddEasyJwt() to set the SigningCredentials or PrivateKey(RSAHelper.Generate() may help you) .";
            public const string UsernameEmptyError = "username can not be null .";
            public const string PasswordValidateError = "password validate fail .";
            public const string InvalidRequest = "invalid request, only post is allowed .";
            public const string InvalidRequestData = "invalid request, only fromform or frombody is allowed .";
            public const string InvalidClientIdOrClientSecret = "invalid client_id or client_secret .";
        }

        internal struct RequestKey
        {
            public const string UserName = "username";
            public const string Password = "password";
            public const string ClientId = "client_id";
            public const string ClientSecret = "client_secret";
        }

        internal static IEnumerable<string> SupportedSigningAlgorithms = new List<string>
        {
            SecurityAlgorithms.RsaSha256,
            SecurityAlgorithms.RsaSha384,
            SecurityAlgorithms.RsaSha512,

            SecurityAlgorithms.RsaSsaPssSha256,
            SecurityAlgorithms.RsaSsaPssSha384,
            SecurityAlgorithms.RsaSsaPssSha512,

            SecurityAlgorithms.EcdsaSha256,
            SecurityAlgorithms.EcdsaSha384,
            SecurityAlgorithms.EcdsaSha512
        };

        internal static class CurveOids
        {
            public const string P256 = "1.2.840.10045.3.1.7";
            public const string P384 = "1.3.132.0.34";
            public const string P521 = "1.3.132.0.35";
        }

        public enum RsaSigningAlgorithm
        {
            RS256,
            RS384,
            RS512,

            PS256,
            PS384,
            PS512
        }

        public enum ECDsaSigningAlgorithm
        {
            ES256,
            ES384,
            ES512
        }

        /// <summary>
        /// Describes the string so we know what to search for in certificate store
        /// </summary>
        public enum NameType
        {
            /// <summary>
            /// subject distinguished name
            /// </summary>
            SubjectDistinguishedName,

            /// <summary>
            /// thumbprint
            /// </summary>
            Thumbprint
        }

        public struct Discovery
        {
            public const string Issuer = "issuer";

            // endpoints
            public const string AuthorizationEndpoint = "authorization_endpoint";
            public const string DeviceAuthorizationEndpoint = "device_authorization_endpoint";
            public const string TokenEndpoint = "token_endpoint";
            public const string UserInfoEndpoint = "userinfo_endpoint";
            public const string IntrospectionEndpoint = "introspection_endpoint";
            public const string RevocationEndpoint = "revocation_endpoint";
            public const string DiscoveryEndpoint = ".well-known/openid-configuration";
            public const string JwksUri = "jwks_uri";
            public const string EndSessionEndpoint = "end_session_endpoint";
            public const string CheckSessionIframe = "check_session_iframe";
            public const string RegistrationEndpoint = "registration_endpoint";
            public const string MtlsEndpointAliases = "mtls_endpoint_aliases";
            public const string PushedAuthorizationRequestEndpoint = "pushed_authorization_request_endpoint";
        }

        public struct LocalApi
        {
            public const string ScopeName = "easy.jwt.local.api";
        }
    }
}