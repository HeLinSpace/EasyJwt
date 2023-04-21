using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using RSAExtensions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Easy.Jwt.Core
{
    public static class RSAHelper
    {
        public static (string publicKey, string privateKey) Generate(RSAKeyType type, bool usePemFormat = false)
        {
            string publicKey, privateKey;

            var rsa = RSA.Create();
            privateKey = rsa.ExportPrivateKey(type, usePemFormat); //私钥
            publicKey = rsa.ExportPublicKey(type, usePemFormat);   //公钥

            return (publicKey, privateKey);
        }

        public static string RSAEncrypt(string content, string publicKey)
        {
            string encryptedContent = string.Empty;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                byte[] encryptedData = rsa.Encrypt(Encoding.Default.GetBytes(content), false);
                encryptedContent = Convert.ToBase64String(encryptedData);
            }
            return encryptedContent;
        }

        public static string RSADecrypt(string content, string private_key)
        {
            string decryptedContent = string.Empty;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(ToXmlPrivateKey(private_key));

                byte[] decryptedData = rsa.Decrypt(Convert.FromBase64String(content), false);
                decryptedContent = Encoding.UTF8.GetString(decryptedData);
            }
            return decryptedContent;
        }

        /// <summary>
        /// base64 private key string -> xml private key
        /// </summary>
        /// <returns></returns>
        public static string ToXmlPrivateKey(string private_key)
        {
            RsaPrivateCrtKeyParameters? privateKeyParams =
                PrivateKeyFactory.CreateKey(Convert.FromBase64String(private_key)) as RsaPrivateCrtKeyParameters;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters rsaParams = new RSAParameters()
                {
                    Modulus = privateKeyParams?.Modulus.ToByteArrayUnsigned(),
                    Exponent = privateKeyParams?.PublicExponent.ToByteArrayUnsigned(),
                    D = privateKeyParams?.Exponent.ToByteArrayUnsigned(),
                    DP = privateKeyParams?.DP.ToByteArrayUnsigned(),
                    DQ = privateKeyParams?.DQ.ToByteArrayUnsigned(),
                    P = privateKeyParams?.P.ToByteArrayUnsigned(),
                    Q = privateKeyParams?.Q.ToByteArrayUnsigned(),
                    InverseQ = privateKeyParams?.QInv.ToByteArrayUnsigned()
                };
                rsa.ImportParameters(rsaParams);
                return rsa.ToXmlString(true);
            }
        }

        /// <summary>
        /// base64 public key string -> xml public key
        /// </summary>
        /// <param name="pubilcKey"></param>
        /// <returns></returns>
        public static string ToXmlPublicKey(string pubilcKey)
        {
            RsaKeyParameters? p =
                PublicKeyFactory.CreateKey(Convert.FromBase64String(pubilcKey)) as RsaKeyParameters;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters rsaParams = new RSAParameters
                {
                    Modulus = p?.Modulus.ToByteArrayUnsigned(),
                    Exponent = p?.Exponent.ToByteArrayUnsigned()
                };
                rsa.ImportParameters(rsaParams);
                return rsa.ToXmlString(false);
            }
        }
    }
}
