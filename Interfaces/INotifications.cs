using System.Collections.Generic;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface INotifications
    {
        bool Enabled { get; set; }
        List<string> AdditionalNotificationEmails { get; set; }
    }
}