using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IReissueResponse
    {
        Result Result { get; set; }
        RegistrationError RegistrationError { get; set; }

    }
}