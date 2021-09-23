using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class CName : ICName
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("value")] public string Value { get; set; }
    }
}
