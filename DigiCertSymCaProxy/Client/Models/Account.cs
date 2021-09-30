using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Account : IAccount
    {
        [JsonProperty("id",NullValueHandling = NullValueHandling.Ignore)] public int Id { get; set; }
    }
}
