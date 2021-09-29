using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IGetCertificateResponse
    {
        CertificateDetails Result { get; set; }
        List<ErrorResponse> CertificateError { get; set; }
    }
}