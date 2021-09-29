using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class DomainComponent : IDomainComponent
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("mandatory")] public bool Mandatory { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("value")] public string Value { get; set; }
    }
}
