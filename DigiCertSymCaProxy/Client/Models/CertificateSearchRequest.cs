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
    public class CertificateSearchRequest : ICertificateSearchRequest
    {
        [JsonProperty("seat_id", NullValueHandling = NullValueHandling.Ignore)] public string SeatId { get; set; }
        [JsonProperty("common_name", NullValueHandling = NullValueHandling.Ignore)] public string CommonName { get; set; }
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)] public string Email { get; set; }
        [JsonProperty("issuing_ca", NullValueHandling = NullValueHandling.Ignore)] public string IssuingCa { get; set; }
        [JsonProperty("profile_id", NullValueHandling = NullValueHandling.Ignore)] public string ProfileId { get; set; }
        [JsonProperty("serial_number", NullValueHandling = NullValueHandling.Ignore)] public string SerialNumber { get; set; }
        [JsonProperty("start_index", NullValueHandling = NullValueHandling.Ignore)] public int StartIndex { get; set; }
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)] public string Status { get; set; }
        [JsonProperty("valid_from", NullValueHandling = NullValueHandling.Ignore)] public string ValidFrom { get; set; }
        [JsonProperty("valid_to", NullValueHandling = NullValueHandling.Ignore)] public string ValidTo { get; set; }
    }
}
