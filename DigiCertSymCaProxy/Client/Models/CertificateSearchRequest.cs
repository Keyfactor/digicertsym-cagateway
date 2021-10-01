using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class CertificateSearchRequest : ICertificateSearchRequest
    {
        [JsonProperty("seat_id", NullValueHandling = NullValueHandling.Ignore)] public string SeatId { get; set; }
        [JsonProperty("common_name", NullValueHandling = NullValueHandling.Ignore)] public string CommonName { get; set; }
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)] public string Email { get; set; }
        [JsonProperty("issuing_ca", NullValueHandling = NullValueHandling.Ignore)] public string IssuingCa { get; set; }
        [JsonProperty("profile_id", NullValueHandling = NullValueHandling.Ignore)] public string ProfileId { get; set; }
        [JsonProperty("serial_number", NullValueHandling = NullValueHandling.Ignore)] public string SerialNumber { get; set; }
        [JsonProperty("start_index", NullValueHandling = NullValueHandling.Ignore)] public int StartIndex { get; set; }
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)] public string Status { get; set; }
        [JsonProperty("valid_from", NullValueHandling = NullValueHandling.Ignore)] public string ValidFrom { get; set; }
        [JsonProperty("valid_to", NullValueHandling = NullValueHandling.Ignore)] public string ValidTo { get; set; }
    }
}
