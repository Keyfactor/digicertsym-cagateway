using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class ErrorList : IErrorList
    {
        public List<ErrorResponse> Errors { get; set; }
    }
}
