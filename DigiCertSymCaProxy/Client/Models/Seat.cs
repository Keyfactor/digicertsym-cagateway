using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Seat : ISeat
    {
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)] public string Email { get; set; }
        [JsonProperty("seat_id", NullValueHandling = NullValueHandling.Ignore)] public string SeatId { get; set; }
        [JsonProperty("seat_name", NullValueHandling = NullValueHandling.Ignore)] public string SeatName { get; set; }
    }
}
