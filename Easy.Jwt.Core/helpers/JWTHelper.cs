using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Easy.Jwt.Core
{
    /// <summary>
    /// JwtToken生成、校验（RSA）
    /// </summary>
    public static class JWTHelper
    {
        /// <summary>
        /// 生成令牌（根据RSA 256私钥创建）
        /// </summary>
        /// <param name="private_key"></param>
        /// <param name="claims"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static string GetJwtTokenRS256(string private_key, IEnumerable<Claim> claims, string issuer, string audience, int expires, out DateTime expiresAt)
        {
            var rsa = RSA.Create();
            rsa.FromXmlString(RSAHelper.ToXmlPrivateKey(private_key));
            SecurityKey securityKey = new RsaSecurityKey(rsa);

            return GetJwtToken(new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256), claims, issuer, audience, expires, out expiresAt);
        }

        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="claims"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static string GetJwtTokenHS256(SymmetricSecurityKey symmetricSecurityKey, IEnumerable<Claim> claims, string issuer, string audience, int expires, out DateTime expiresAt)
        {
            return GetJwtToken(new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256), claims, issuer, audience, expires, out expiresAt);
        }

        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="claims"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static string GetJwtToken(SigningCredentials credentials, IEnumerable<Claim> claims, string issuer, string audience, int expires, out DateTime expiresAt)
        {
            expiresAt = DateTime.Now.AddSeconds(expires);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 通过公钥创建私钥
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static RsaSecurityKey GetRsaSecurityKey(string publicKey)
        {
            return new RsaSecurityKey(CreateRsaProviderFromPublicKey(publicKey));
        }

        /// <summary>
        /// 检查令牌是否过期
        /// </summary>
        /// <param name="token"></param>
        /// <param name="preMinutes"></param>
        /// <returns>DateTime.Now.AddMinutes(-1 * preMinutes) >= expAt</returns>
        public static bool NeedRefresh(string token, int preMinutes = 0)
        {
            var expAt = GetExpAt(token);
            if (expAt != null)
            {
                return DateTime.Now.AddMinutes(-1 * preMinutes) >= expAt;
            }

            return false;
        }

        /// <summary>
        /// 读取令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static JwtSecurityToken ReadJwtToken(string token)
        {
            try
            {
                return new JwtSecurityTokenHandler().ReadJwtToken(token);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据令牌获取Claims信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> GetClaims(string token)
        {
            return ReadJwtToken(token)?.Claims;
        }

        public static string GetClaimValue(this IEnumerable<Claim> claims, string key, StringComparison ignoreCase = StringComparison.OrdinalIgnoreCase)
        {
            return claims?.FirstOrDefault(s => s.Type.Equals(key, ignoreCase))?.Value;
        }

        /// <summary>
        /// 获取令牌过期时间
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DateTime? GetExpAt(string token)
        {
            var exp = GetClaims(token)?.GetClaimValue("exp");

            if (DateTime.TryParse(exp, out DateTime result))
            {
                return result;
            }

            return null;
        }

        public static RSA GetRsaProviderFromPublicKey(string publicKey)
        {
            return CreateRsaProviderFromPublicKey(publicKey);
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token"></param>
        /// <param name="issuerSigningKey"></param>
        /// <returns></returns>
        public static JwtValidateResult VerifyJwtToken(string token, string issuerSigningKey, string issuer, string audience)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey));
            return VerifyJwtToken(token, key, issuer, audience);
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static JwtValidateResult VerifyJwtTokenRSA(string token, string publicKey, string issuer, string audience)
        {
            return VerifyJwtToken(token, new RsaSecurityKey(CreateRsaProviderFromPublicKey(publicKey)),  issuer,  audience);
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static JwtValidateResult VerifyJwtToken(string token, SecurityKey key, string issuer, string audience)
        {
            var result = new JwtValidateResult();

            //校验token
            var validateParameter = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = key,

                ClockSkew = TimeSpan.Zero//校验过期时间必须加此属性
            };

            try
            {
                //校验并解析token,validatedToken是解密后的对象
                result.Principal = new JwtSecurityTokenHandler().ValidateToken(token, validateParameter, out SecurityToken validatedToken);
                var exp = result.Principal.Claims?.GetClaimValue("exp");

                if (DateTime.TryParse(exp, out DateTime expAt))
                {
                     result.ExpAt = expAt;
                }

                result.Success = true;
            }
            catch (SecurityTokenExpiredException)
            {
                result.ErrorType = JwtValidateError.ExpiredError;
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                result.ErrorType = JwtValidateError.AudienceError;
            }
            catch (SecurityTokenException)
            {
                result.ErrorType = JwtValidateError.TokenError;
            }
            catch
            {
                result.ErrorType = JwtValidateError.UnKnownError;
            }

            return result;
        }

        #region 私有方法

        /// <summary>
        /// 通过公钥创建RSA
        /// </summary>
        /// <param name="publicKeyString"></param>
        /// <returns></returns>
        private static RSA CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            var x509Key = Convert.FromBase64String(publicKeyString);
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (MemoryStream mem = new MemoryStream(x509Key))
            {
                using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    seq = binr.ReadBytes(15);       //read the Sequence OID
                    if (!CompareBytearrays(seq, seqOid))    //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;
                    int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    var rsa = RSA.Create();
                    RSAParameters rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };

                    rsa.ImportParameters(rsaKeyInfo);

                    return rsa;
                }
            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        #endregion
    }

    public class JwtValidateResult
    {
        public ClaimsPrincipal Principal { get; set; }

        public bool Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpAt { get; set; }
        
        public JwtValidateError? ErrorType { get; set; }
    }
}