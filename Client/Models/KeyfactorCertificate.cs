using System;
using Keyfactor.AnyGateway.CscGlobal.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.CscGlobal.Client.Models
{
    public class KeyfactorCertificate : IKeyfactorCertificate
    {
        public int Id { get; set; }
        public string Thumbprint { get; set; }
        public string SerialNumber { get; set; }
        [JsonProperty("IssuedDN")] public string IssuedDn { get; set; }
        [JsonProperty("IssuedCN")] public string IssuedCn { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
        [JsonProperty("IssuerDN")] public string IssuerDn { get; set; }
        public string PrincipalId { get; set; }
        [JsonProperty("TemplateId", NullValueHandling = NullValueHandling.Ignore)] public int TemplateId { get; set; }
        public int CertState { get; set; }
        public int KeySizeInBits { get; set; }
        public int KeyType { get; set; }
        public string RequesterId { get; set; }
    }
}
