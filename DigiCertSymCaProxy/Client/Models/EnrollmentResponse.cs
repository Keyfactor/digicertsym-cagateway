using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class EnrollmentResponse : IEnrollmentResponse
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)] public EnrollmentSuccessResponse Result { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ErrorList RegistrationError { get; set; }
    }
}
