using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Validity : IValidity
    {
        [JsonProperty("duration")] public int Duration { get; set; }
        [JsonProperty("unit")] public string Unit { get; set; }
    }
}
