using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class DomainControlValidation : IDomainControlValidation
    {
        [JsonProperty("methodType")] public string MethodType { get; set; }
        [JsonProperty("emailAddress")] public string EmailAddress { get; set; }
    }
}
