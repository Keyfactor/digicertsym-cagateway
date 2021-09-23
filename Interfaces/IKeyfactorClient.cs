using System.Collections.Generic;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IKeyfactorClient
    {

        Task<List<KeyfactorCertificate>> SubmitGetKeyfactorCertAsync(string thumbprintFilter);
    }
}
