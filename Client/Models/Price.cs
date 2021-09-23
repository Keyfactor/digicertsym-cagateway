using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Price : IPrice
    {
        [JsonProperty("currency")] public string Currency { get; set; }
        [JsonProperty("total")] public decimal Total { get; set; }
    }
}
