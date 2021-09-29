using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class EnrollmentRequest : IEnrollmentRequest
    {
        [JsonProperty("attributes")] public Attributes Attributes { get; set; }
        [JsonProperty("authentication")] public Authentication Authentication { get; set; }
        [JsonProperty("csr")] public string Csr { get; set; }
        [JsonProperty("profile")] public Profile Profile { get; set; }
        [JsonProperty("seat")] public Seat Seat { get; set; }
        [JsonProperty("session_key")] public string SessionKey { get; set; }
        [JsonProperty("validity")] public Validity Validity { get; set; }
    }
}
