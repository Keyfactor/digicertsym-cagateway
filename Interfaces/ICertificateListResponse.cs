using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ICertificateListResponse
    {
        Meta Meta { get; set; }
        List<CertificateResponse> Results { get; set; }
    }
}