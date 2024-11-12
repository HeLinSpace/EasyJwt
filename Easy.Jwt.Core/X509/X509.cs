using System.Security.Cryptography.X509Certificates;

namespace Easy.Jwt.Core
{
    internal static class X509
    {
        public static X509CertificatesLocation CurrentUser => new(StoreLocation.CurrentUser);
        public static X509CertificatesLocation LocalMachine => new(StoreLocation.LocalMachine);
    }
}
