using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class CustomAttributes : ICustomAttributes
    {
        [JsonProperty("additionalProp1", NullValueHandling = NullValueHandling.Ignore)] public string AdditionalProp1 { get; set; }
        [JsonProperty("additionalProp2", NullValueHandling = NullValueHandling.Ignore)] public string AdditionalProp2 { get; set; }
        [JsonProperty("additionalProp3", NullValueHandling = NullValueHandling.Ignore)] public string AdditionalProp3 { get; set; }
    }
}
