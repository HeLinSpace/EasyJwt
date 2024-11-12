using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Easy.Jwt.Core
{
    internal class X509CertificatesFinder
    {
        readonly StoreLocation _location;
        readonly StoreName _name;
        readonly X509FindType _findType;

        public X509CertificatesFinder(StoreLocation location, StoreName name, X509FindType findType)
        {
            _location = location;
            _name = name;
            _findType = findType;
        }

        public IEnumerable<X509Certificate2> Find(object findValue, bool validOnly = true)
        {
#if NET452
            var store = new X509Store(_name, _location);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var certColl = store.Certificates.Find(_findType, findValue, validOnly);
                store.Close();
                return certColl.Cast<X509Certificate2>();
            }
            finally
            {
                store.Close();
            }
#else
            using var store = new X509Store(_name, _location);
            store.Open(OpenFlags.ReadOnly);

            var certColl = store.Certificates.Find(_findType, findValue, validOnly);
            return certColl.Cast<X509Certificate2>();
#endif
        }
    }
}
