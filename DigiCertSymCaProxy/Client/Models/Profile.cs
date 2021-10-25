using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Profile : IProfile
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)] public string Id { get; set; }
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)] public string Name { get; set; }
    }
}
