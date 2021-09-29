using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class CustomAttributes : ICustomAttributes
    {
        [JsonProperty("additionalProp1")] public string AdditionalProp1 { get; set; }
        [JsonProperty("additionalProp2")] public string AdditionalProp2 { get; set; }
        [JsonProperty("additionalProp3")] public string AdditionalProp3 { get; set; }
    }
}
