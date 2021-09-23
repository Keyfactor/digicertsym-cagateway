using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ISubjectAlternativeName
    {
        string DomainName { get; set; }
        DomainControlValidation DomainControlValidation { get; set; }
    }
}