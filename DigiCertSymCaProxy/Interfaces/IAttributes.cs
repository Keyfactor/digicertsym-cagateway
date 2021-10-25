using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IAttributes
    {
        string CommonName { get; set; }
        string ContentType { get; set; }
        string CounterSignature { get; set; }
        string Country { get; set; }
        CustomAttributes CustomAttributes { get; set; }
        string DnQualifier { get; set; }
        List<DomainComponent> DomainComponent { get; set; }
        string DomainName { get; set; }
        string Email { get; set; }
        string GivenName { get; set; }
        string IpAddress { get; set; }
        string JobTitle { get; set; }
        string Locality { get; set; }
        string MessageDigest { get; set; }
        string OrganizationName { get; set; }
        List<OrganizationUnit> OrganizationUnit { get; set; }
        string PostalCode { get; set; }
        string Pseudonym { get; set; }
        San San { get; set; }
        string SerialNumber { get; set; }
        string SigningTime { get; set; }
        string State { get; set; }
        List<StreetAddress> StreetAddress { get; set; }
        string Surname { get; set; }
        string UniqueIdentifier { get; set; }
        string UnstructuredAddress { get; set; }
        string UnstructuredName { get; set; }
        string UserId { get; set; }
    }
}