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
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ISan
    {
        CustomAttributes CustomAttributes { get; set; }
        string DirectoryName { get; set; }
        List<DnsName> DnsName { get; set; }
        List<IpAddress> IpAddress { get; set; }
        List<OtherName> OtherName { get; set; }
        List<RegisteredId> RegisteredId { get; set; }
        List<Rfc822Name> Rfc822Name { get; set; }
        List<UserPrincipalName> UserPrincipalName { get; set; }
    }
}