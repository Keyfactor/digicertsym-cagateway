// Copyright 2023 Keyfactor                                                   
// Licensed under the Apache License, Version 2.0 (the "License"); you may    
// not use this file except in compliance with the License.  You may obtain a 
// copy of the License at http://www.apache.org/licenses/LICENSE-2.0.  Unless 
// required by applicable law or agreed to in writing, software distributed   
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES   
// OR CONDITIONS OF ANY KIND, either express or implied. See the License for  
// thespecific language governing permissions and limitations under the       
// License. 
﻿using System;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class CertificateDetails : ICertificateDetails
    {
        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)] public Profile Profile { get; set; }
        [JsonProperty("seat", NullValueHandling = NullValueHandling.Ignore)] public Seat Seat { get; set; }
        [JsonProperty("account", NullValueHandling = NullValueHandling.Ignore)] public Account Account { get; set; }
        [JsonProperty("certificate", NullValueHandling = NullValueHandling.Ignore)] public string Certificate { get; set; }
        [JsonProperty("common_name", NullValueHandling = NullValueHandling.Ignore)] public string CommonName { get; set; }
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)] public string Status { get; set; }
        [JsonProperty("serial_number", NullValueHandling = NullValueHandling.Ignore)] public string SerialNumber { get; set; }
        [JsonProperty("valid_from", NullValueHandling = NullValueHandling.Ignore)] public DateTime ValidFrom { get; set; }
        [JsonProperty("valid_to", NullValueHandling = NullValueHandling.Ignore)] public DateTime ValidTo { get; set; }
        [JsonProperty("is_key_escrowed", NullValueHandling = NullValueHandling.Ignore)] public bool IsKeyEscrowed { get; set; }
        [JsonProperty("enrollment_notes", NullValueHandling = NullValueHandling.Ignore)] public string EnrollmentNotes { get; set; }
    }
}
