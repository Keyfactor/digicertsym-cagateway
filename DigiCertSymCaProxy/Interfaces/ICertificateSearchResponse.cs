using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ICertificateSearchResponse
    {
        int Count { get; set; }
        bool MoreCertsAvailable { get; set; }
        int Index { get; set; }
        List<CertificateDetails> Certificates { get; set; }
    }
}