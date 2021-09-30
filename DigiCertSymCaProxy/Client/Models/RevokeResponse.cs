using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class RevokeResponse : IRevokeResponse
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)] public string Result { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ErrorList RegistrationError { get; set; }
    }
}
