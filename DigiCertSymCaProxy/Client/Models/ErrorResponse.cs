using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class ErrorResponse : IErrorResponse
    {
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)] public string Code { get; set; }
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)] public string Message { get; set; }
        [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)] public string Field { get; set; }
    }
}
