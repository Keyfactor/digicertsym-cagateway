﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Authentication:IAuthentication
    {
        [JsonProperty("additionalProp1")] public string AdditionalProp1 { get; set; }
        [JsonProperty("additionalProp2")]  public string AdditionalProp2 { get; set; }
        [JsonProperty("additionalProp3")]  public string AdditionalProp3 { get; set; }
    }
}