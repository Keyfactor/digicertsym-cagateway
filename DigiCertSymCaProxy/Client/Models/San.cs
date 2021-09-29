using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class San : ISan
    {
        [JsonProperty("custom_attributes")] public CustomAttributes CustomAttributes { get; set; }
        [JsonProperty("directory_name")] public string DirectoryName { get; set; }
        [JsonProperty("dns_name")] public List<DnsName> DnsName { get; set; }
        [JsonProperty("ip_address")] public List<IpAddress> IpAddress { get; set; }
        [JsonProperty("other_name")] public List<OtherName> OtherName { get; set; }
        [JsonProperty("registered_id")] public List<RegisteredId> RegisteredId { get; set; }
        [JsonProperty("rfc822_name")] public List<Rfc822Name> Rfc822Name { get; set; }
        [JsonProperty("user_principal_name")] public List<UserPrincipalName> UserPrincipalName { get; set; }
    }
}
