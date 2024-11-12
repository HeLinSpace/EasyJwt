using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    internal class X509CertificatesLocation
    {
        readonly StoreLocation _location;

        public X509CertificatesLocation(StoreLocation location)
        {
            _location = location;
        }

        public X509CertificatesName My => new X509CertificatesName(_location, StoreName.My);
        public X509CertificatesName AddressBook => new X509CertificatesName(_location, StoreName.AddressBook);
        public X509CertificatesName TrustedPeople => new X509CertificatesName(_location, StoreName.TrustedPeople);
        public X509CertificatesName TrustedPublisher => new X509CertificatesName(_location, StoreName.TrustedPublisher);
        public X509CertificatesName CertificateAuthority => new X509CertificatesName(_location, StoreName.CertificateAuthority);
    }
}
