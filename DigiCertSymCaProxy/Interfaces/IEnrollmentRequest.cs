using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IEnrollmentRequest
    {
        Attributes Attributes { get; set; }
        Authentication Authentication { get; set; }
        string Csr { get; set; }
        Profile Profile { get; set; }
        Seat Seat { get; set; }
        string SessionKey { get; set; }
        Validity Validity { get; set; }
    }
}