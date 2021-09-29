using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ISan
    {
        CustomAttributes CustomAttributes { get; set; }
        string DirectoryName { get; set; }
        List<DnsName> DnsName { get; set; }
        List<IpAddress> IpAddress { get; set; }
        List<OtherName> OtherName { get; set; }
        List<RegisteredId> RegisteredId { get; set; }
        List<Rfc822Name> Rfc822Name { get; set; }
        List<UserPrincipalName> UserPrincipalName { get; set; }
    }
}