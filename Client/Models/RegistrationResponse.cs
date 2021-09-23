using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class RegistrationResponse : IRegistrationResponse
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)] public Result Result { get; set; }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)] public RegistrationError RegistrationError { get; set; }
    }
}
