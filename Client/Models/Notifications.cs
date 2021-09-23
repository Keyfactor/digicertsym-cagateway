using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Notifications : INotifications
    {
        [JsonProperty("enabled")] public bool Enabled { get; set; }
        [JsonProperty("additionalNotificationEmails")] public List<string> AdditionalNotificationEmails { get; set; }
    }
}
