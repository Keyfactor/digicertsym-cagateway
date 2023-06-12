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
    public class EnrollmentSuccessResponse : IEnrollmentSuccessResponse
    {
        [JsonProperty("serial_number", NullValueHandling = NullValueHandling.Ignore)] public string SerialNumber { get; set; }
        [JsonProperty("delivery_format", NullValueHandling = NullValueHandling.Ignore)] public string DeliveryFormat { get; set; }
        [JsonProperty("certificate", NullValueHandling = NullValueHandling.Ignore)] public string Certificate { get; set; }
        [JsonProperty("pkcs12_password", NullValueHandling = NullValueHandling.Ignore)] public string Pkcs12Password { get; set; }
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)] public string Status { get; set; }
    }
}
