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
    public interface IAttributes
    {
        string CommonName { get; set; }
        string ContentType { get; set; }
        string CounterSignature { get; set; }
        string Country { get; set; }
        CustomAttributes CustomAttributes { get; set; }
        string DnQualifier { get; set; }
        List<DomainComponent> DomainComponent { get; set; }
        string DomainName { get; set; }
        string Email { get; set; }
        string GivenName { get; set; }
        string IpAddress { get; set; }
        string JobTitle { get; set; }
        string Locality { get; set; }
        string MessageDigest { get; set; }
        string OrganizationName { get; set; }
        List<OrganizationUnit> OrganizationUnit { get; set; }
        string PostalCode { get; set; }
        string Pseudonym { get; set; }
        San San { get; set; }
        string SerialNumber { get; set; }
        string SigningTime { get; set; }
        string State { get; set; }
        List<StreetAddress> StreetAddress { get; set; }
        string Surname { get; set; }
        string UniqueIdentifier { get; set; }
        string UnstructuredAddress { get; set; }
        string UnstructuredName { get; set; }
        string UserId { get; set; }
    }
}