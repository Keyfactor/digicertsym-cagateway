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
        [JsonProperty("custom_attributes", NullValueHandling = NullValueHandling.Ignore)] public CustomAttributes CustomAttributes { get; set; }
        [JsonProperty("directory_name", NullValueHandling = NullValueHandling.Ignore)] public string DirectoryName { get; set; }
        [JsonProperty("dns_name", NullValueHandling = NullValueHandling.Ignore)] public List<DnsName> DnsName { get; set; }
        [JsonProperty("ip_address", NullValueHandling = NullValueHandling.Ignore)] public List<IpAddress> IpAddress { get; set; }
        [JsonProperty("other_name", NullValueHandling = NullValueHandling.Ignore)] public List<OtherName> OtherName { get; set; }
        [JsonProperty("registered_id", NullValueHandling = NullValueHandling.Ignore)] public List<RegisteredId> RegisteredId { get; set; }
        [JsonProperty("rfc822_name", NullValueHandling = NullValueHandling.Ignore)] public List<Rfc822Name> Rfc822Name { get; set; }
        [JsonProperty("user_principal_name", NullValueHandling = NullValueHandling.Ignore)] public List<UserPrincipalName> UserPrincipalName { get; set; }
    }
}
