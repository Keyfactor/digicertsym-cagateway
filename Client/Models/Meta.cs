using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Meta : IMeta
    {
        [JsonProperty("numResults")] public int NumResults { get; set; }
    }
}
