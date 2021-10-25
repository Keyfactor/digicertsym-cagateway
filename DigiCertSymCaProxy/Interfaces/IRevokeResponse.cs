using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IRevokeResponse
    {
        string Result { get; set; }
        ErrorList RegistrationError { get; set; }
    }
}