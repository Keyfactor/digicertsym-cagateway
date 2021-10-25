using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class GetCertificateResponse : IGetCertificateResponse
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)] public CertificateDetails Result { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ErrorList CertificateError { get; set; }
    }
}
