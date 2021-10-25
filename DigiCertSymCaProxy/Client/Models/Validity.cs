using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Validity : IValidity
    {
        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)] public int Duration { get; set; }
        [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)] public string Unit { get; set; }
    }
}
