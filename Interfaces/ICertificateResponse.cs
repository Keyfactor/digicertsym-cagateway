using System.Collections.Generic;
using Keyfactor.AnyGateway.CscGlobal.Client.Models;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.CscGlobal.Interfaces
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