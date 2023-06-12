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
    public class KeyfactorCertificate : IKeyfactorCertificate
    {
        public int Id { get; set; }
        public string Thumbprint { get; set; }
        public string SerialNumber { get; set; }
        [JsonProperty("IssuedDN")] public string IssuedDn { get; set; }
        [JsonProperty("IssuedCN")] public string IssuedCn { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
        [JsonProperty("IssuerDN")] public string IssuerDn { get; set; }
        public string PrincipalId { get; set; }
        [JsonProperty("TemplateId", NullValueHandling = NullValueHandling.Ignore)] public int TemplateId { get; set; }
        public int CertState { get; set; }
        public int KeySizeInBits { get; set; }
        public int KeyType { get; set; }
        public string RequesterId { get; set; }
    }
}
