using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Rfc822Name : IRfc822Name
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)] public string Id { get; set; }
        [JsonProperty("mandatory", NullValueHandling = NullValueHandling.Ignore)] public bool Mandatory { get; set; }
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)] public string Type { get; set; }
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)] public string Value { get; set; }
    }
}
