using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IRenewalResponse
    {
        Result Result { get; set; }
        RegistrationError RegistrationError { get; set; }

    }
}