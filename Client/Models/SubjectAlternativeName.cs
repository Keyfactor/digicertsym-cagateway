using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class SubjectAlternativeName : ISubjectAlternativeName
    {
        [JsonProperty("domainName")] public string DomainName { get; set; }
        [JsonProperty("domainControlValidation")] public DomainControlValidation DomainControlValidation { get; set; }
    }
}
