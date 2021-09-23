using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CertificateListResponse : ICertificateListResponse
    {
        [JsonProperty("meta")] public Meta Meta { get; set; }
        [JsonProperty("results")] public List<CertificateResponse> Results { get; set; }
    }

}
