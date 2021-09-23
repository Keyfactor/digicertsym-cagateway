using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IRegistrationResponse
    {
        Result Result { get; set; }
        RegistrationError RegistrationError { get; set; }
    }
}