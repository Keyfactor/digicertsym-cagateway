using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IGetCertificateResponse
    {
        CertificateDetails Result { get; set; }
        ErrorList CertificateError { get; set; }
    }
}