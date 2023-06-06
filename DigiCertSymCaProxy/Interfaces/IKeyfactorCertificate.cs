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

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IKeyfactorCertificate
    {
        int Id { get; set; }
        string Thumbprint { get; set; }
        string SerialNumber { get; set; }
        string IssuedDn { get; set; }
        string IssuedCn { get; set; }
        DateTime NotBefore { get; set; }
        DateTime NotAfter { get; set; }
        string IssuerDn { get; set; }
        string PrincipalId { get; set; }
        int TemplateId { get; set; }
        int CertState { get; set; }
        int KeySizeInBits { get; set; }
        int KeyType { get; set; }
        string RequesterId { get; set; }
    }
}