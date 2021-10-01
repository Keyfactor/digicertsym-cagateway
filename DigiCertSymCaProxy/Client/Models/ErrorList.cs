using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class ErrorList : IErrorList
    {
        public List<ErrorResponse> Errors { get; set; }
    }
}
