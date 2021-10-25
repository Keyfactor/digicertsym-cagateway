using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class EnrollmentSuccessResponse : IEnrollmentSuccessResponse
    {
        [JsonProperty("serial_number", NullValueHandling = NullValueHandling.Ignore)] public string SerialNumber { get; set; }
        [JsonProperty("delivery_format", NullValueHandling = NullValueHandling.Ignore)] public string DeliveryFormat { get; set; }
        [JsonProperty("certificate", NullValueHandling = NullValueHandling.Ignore)] public string Certificate { get; set; }
        [JsonProperty("pkcs12_password", NullValueHandling = NullValueHandling.Ignore)] public string Pkcs12Password { get; set; }
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)] public string Status { get; set; }
    }
}
