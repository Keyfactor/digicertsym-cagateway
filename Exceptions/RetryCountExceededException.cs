using System;

namespace Keyfactor.AnyGateway.DigiCertSym.Exceptions
{
    public class RetryCountExceededException : Exception
    {
        public RetryCountExceededException(string message) : base(message)
        {
        }
    }
}