using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class RevokeRequest : IRevokeRequest
    {
        [JsonProperty("revocation_reason", NullValueHandling = NullValueHandling.Ignore)] public string RevocationReason { get; set; }
    }
}
