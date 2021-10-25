using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class DomainComponent : IDomainComponent
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)] public string Id { get; set; }
        [JsonProperty("mandatory", NullValueHandling = NullValueHandling.Ignore)] public bool Mandatory { get; set; }
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)] public string Type { get; set; }
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)] public string Value { get; set; }
    }
}
