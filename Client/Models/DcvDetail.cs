using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class DcvDetail : IDcvDetail
    {
        [JsonProperty("domainName")] public string DomainName { get; set; }
        [JsonProperty("actionNeeded")] public string ActionNeeded { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("cname")] public CName CName { get; set; }
    }
}
