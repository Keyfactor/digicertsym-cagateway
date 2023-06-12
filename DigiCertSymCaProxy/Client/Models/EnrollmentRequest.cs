// Copyright 2023 Keyfactor                                                   
// Licensed under the Apache License, Version 2.0 (the "License"); you may    
// not use this file except in compliance with the License.  You may obtain a 
// copy of the License at http://www.apache.org/licenses/LICENSE-2.0.  Unless 
// required by applicable law or agreed to in writing, software distributed   
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES   
// OR CONDITIONS OF ANY KIND, either express or implied. See the License for  
// thespecific language governing permissions and limitations under the       
// License. 
﻿using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class EnrollmentRequest : IEnrollmentRequest
    {
        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)] public Attributes Attributes { get; set; }
        [JsonProperty("authentication", NullValueHandling = NullValueHandling.Ignore)] public Authentication Authentication { get; set; }
        [JsonProperty("csr", NullValueHandling = NullValueHandling.Ignore)] public string Csr { get; set; }
        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)] public Profile Profile { get; set; }
        [JsonProperty("seat", NullValueHandling = NullValueHandling.Ignore)] public Seat Seat { get; set; }
        [JsonProperty("session_key", NullValueHandling = NullValueHandling.Ignore)] public string SessionKey { get; set; }
        [JsonProperty("validity", NullValueHandling = NullValueHandling.Ignore)] public Validity Validity { get; set; }
    }
}
