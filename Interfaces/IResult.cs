using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IResult
    {
        string CommonName { get; set; }
        Status Status { get; set; }
        string CertificateType { get; set; }
        Price Price { get; set; }
        List<DcvDetail> DcvDetails { get; set; }
    }
}