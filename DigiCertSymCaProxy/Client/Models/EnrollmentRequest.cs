using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class EnrollmentRequest : IEnrollmentRequest
    {
        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)] public Attributes Attributes { get; set; }
        [JsonProperty("authentication", NullValueHandling = NullValueHandling.Ignore)] public Authentication Authentication { get; set; }
        [JsonProperty("csr", NullValueHandling = NullValueHandling.Ignore)] public string Csr { get; set; }
        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)] public Profile Profile { get; set; }
        [JsonProperty("seat", NullValueHandling = NullValueHandling.Ignore)] public Seat Seat { get; set; }
        [JsonProperty("session_key", NullValueHandling = NullValueHandling.Ignore)] public string SessionKey { get; set; }
        [JsonProperty("validity", NullValueHandling = NullValueHandling.Ignore)] public Validity Validity { get; set; }
    }
}
