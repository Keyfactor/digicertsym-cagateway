using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IDcvDetail
    {
        string DomainName { get; set; }
        string ActionNeeded { get; set; }
        string Email { get; set; }
        CName CName { get; set; }
    }
}