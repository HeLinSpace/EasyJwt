using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Jwt.Core
{
    internal class X509CertificatesName
    {
        readonly StoreLocation _location;
        readonly StoreName _name;

        public X509CertificatesName(StoreLocation location, StoreName name)
        {
            _location = location;
            _name = name;
        }

        public X509CertificatesFinder Thumbprint => new(_location, _name, X509FindType.FindByThumbprint);
        public X509CertificatesFinder SubjectDistinguishedName => new(_location, _name, X509FindType.FindBySubjectDistinguishedName);
        public X509CertificatesFinder SerialNumber => new(_location, _name, X509FindType.FindBySerialNumber);
        public X509CertificatesFinder IssuerName => new(_location, _name, X509FindType.FindByIssuerName);
    }
}
