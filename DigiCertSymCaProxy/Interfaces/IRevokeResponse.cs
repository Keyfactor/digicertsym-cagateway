using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IRevokeResponse
    {
        string Result { get; set; }
        List<ErrorResponse> RegistrationError { get; set; }
    }
}