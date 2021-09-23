using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class RegistrationError : IRegistrationError
    {
        [JsonProperty("code")] public string Code { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("value")] public string Value { get; set; }

    }
}
