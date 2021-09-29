using System;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class CertificateDetails : ICertificateDetails
    {
        [JsonProperty("profile")] public Profile Profile { get; set; }
        [JsonProperty("seat")] public Seat Seat { get; set; }
        [JsonProperty("account")] public Account Account { get; set; }
        [JsonProperty("certificate")] public string Certificate { get; set; }
        [JsonProperty("common_name")] public string CommonName { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("serial_number")] public string SerialNumber { get; set; }
        [JsonProperty("valid_from")] public DateTime ValidFrom { get; set; }
        [JsonProperty("valid_to")] public DateTime ValidTo { get; set; }
        [JsonProperty("is_key_escrowed")] public bool IsKeyEscrowed { get; set; }
        [JsonProperty("enrollment_notes")] public string EnrollmentNotes { get; set; }
    }
}
