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
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("seat_id")] public string SeatId { get; set; }
        [JsonProperty("seat_name")] public string SeatName { get; set; }
    }
}
