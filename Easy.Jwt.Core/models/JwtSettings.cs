using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;

namespace Easy.Jwt.Core
{
    public class JwtSettings
    {
        /// <summary>
        /// default in payload
        /// </summary>
        public IEnumerable<Claim> DefaultClaims { get; set; }

        /// <summary>
        /// issuer
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// audience
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// expires on（seconds），default 8 hours
        /// </summary>
        public int Expires { get; set; } = 28800;

        /// <summary>
        /// the security key to generate jwt. if not configured, rsa will be used
        /// </summary>
        public string IssuerSigningKey { get; set; }

        /// <summary>
        /// the rsa private key .if not configured, use default
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// token类型 default Bearer
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
    }
}
