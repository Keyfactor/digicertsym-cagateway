using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.CscGlobal.Client.Models;

namespace Keyfactor.AnyGateway.CscGlobal.Interfaces
{
    public interface IKeyfactorClient
    {

        Task<List<KeyfactorCertificate>> SubmitGetKeyfactorCertAsync(string thumbprintFilter);
    }
}
