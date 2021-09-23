using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class RevokeSuccessResponse
    {
        [JsonProperty("commonName")] public string CommonName { get; set; }
        [JsonProperty("certificateType")] public string CertificateType { get; set; }
        [JsonProperty("status")] public string Status { get; set; }

    }
}
