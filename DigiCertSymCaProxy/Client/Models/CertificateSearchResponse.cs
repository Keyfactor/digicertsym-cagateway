using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class CertificateSearchResponse : ICertificateSearchResponse
    {
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)] public int Count { get; set; }
        [JsonProperty("more_certs_available", NullValueHandling = NullValueHandling.Ignore)] public bool MoreCertsAvailable { get; set; }
        [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)] public int Index { get; set; }
        [JsonProperty("certificates", NullValueHandling = NullValueHandling.Ignore)] public List<CertificateDetails> Certificates { get; set; }
    }
}
