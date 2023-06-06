// Copyright 2023 Keyfactor                                                   
// Licensed under the Apache License, Version 2.0 (the "License"); you may    
// not use this file except in compliance with the License.  You may obtain a 
// copy of the License at http://www.apache.org/licenses/LICENSE-2.0.  Unless 
// required by applicable law or agreed to in writing, software distributed   
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES   
// OR CONDITIONS OF ANY KIND, either express or implied. See the License for  
// thespecific language governing permissions and limitations under the       
// License. 
﻿using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class San : ISan
    {
        [JsonProperty("custom_attributes", NullValueHandling = NullValueHandling.Ignore)] public CustomAttributes CustomAttributes { get; set; }
        [JsonProperty("directory_name", NullValueHandling = NullValueHandling.Ignore)] public string DirectoryName { get; set; }
        [JsonProperty("dns_name", NullValueHandling = NullValueHandling.Ignore)] public List<DnsName> DnsName { get; set; }
        [JsonProperty("ip_address", NullValueHandling = NullValueHandling.Ignore)] public List<IpAddress> IpAddress { get; set; }
        [JsonProperty("other_name", NullValueHandling = NullValueHandling.Ignore)] public List<OtherName> OtherName { get; set; }
        [JsonProperty("registered_id", NullValueHandling = NullValueHandling.Ignore)] public List<RegisteredId> RegisteredId { get; set; }
        [JsonProperty("rfc822_name", NullValueHandling = NullValueHandling.Ignore)] public List<Rfc822Name> Rfc822Name { get; set; }
        [JsonProperty("user_principal_name", NullValueHandling = NullValueHandling.Ignore)] public List<UserPrincipalName> UserPrincipalName { get; set; }
    }
}
