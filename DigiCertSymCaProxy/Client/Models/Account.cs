using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Account : IAccount
    {
        [JsonProperty("id",NullValueHandling = NullValueHandling.Ignore)] public int Id { get; set; }
    }
}
