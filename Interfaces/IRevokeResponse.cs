using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IRevokeResponse
    {
        RevokeSuccessResponse RevokeSuccess { get; set; }
        RegistrationError RegistrationError { get; set; }
    }
}