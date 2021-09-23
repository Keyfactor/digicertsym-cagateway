using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ICertificateResponse
    {
        string Uuid { get; set; }
        string CommonName { get; set; }
        List<string> AdditionalNames { get; set; }
        string CertificateType { get; set; }
        string Status { get; set; }
        string EffectiveDate { get; set; }
        string ExpirationDate { get; set; }
        string BusinessUnit { get; set; }
        string OrderedBy { get; set; }
        string OrderDate { get; set; }
        object ServerSoftware { get; set; }
        string Certificate { get; set; }
        List<CustomField> CustomFields { get; set; }
    }
}