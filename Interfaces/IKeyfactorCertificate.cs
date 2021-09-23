using System;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IKeyfactorCertificate
    {
        int Id { get; set; }
        string Thumbprint { get; set; }
        string SerialNumber { get; set; }
        string IssuedDn { get; set; }
        string IssuedCn { get; set; }
        DateTime NotBefore { get; set; }
        DateTime NotAfter { get; set; }
        string IssuerDn { get; set; }
        string PrincipalId { get; set; }
        int TemplateId { get; set; }
        int CertState { get; set; }
        int KeySizeInBits { get; set; }
        int KeyType { get; set; }
        string RequesterId { get; set; }
    }
}