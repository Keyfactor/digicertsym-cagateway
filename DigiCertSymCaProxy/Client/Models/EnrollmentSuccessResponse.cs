using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class EnrollmentSuccessResponse : IEnrollmentSuccessResponse
    {
        [JsonProperty("serial_number")] public string SerialNumber { get; set; }
        [JsonProperty("delivery_format")] public string DeliveryFormat { get; set; }
        [JsonProperty("certificate")] public string Certificate { get; set; }
        [JsonProperty("pkcs12_password")] public string Pkcs12Password { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
    }
}
