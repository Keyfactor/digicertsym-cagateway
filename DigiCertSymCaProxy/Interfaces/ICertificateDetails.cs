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
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ICertificateDetails
    {
        Profile Profile { get; set; }
        Seat Seat { get; set; }
        Account Account { get; set; }
        string Certificate { get; set; }
        string CommonName { get; set; }
        string Status { get; set; }
        string SerialNumber { get; set; }
        DateTime ValidFrom { get; set; }
        DateTime ValidTo { get; set; }
        bool IsKeyEscrowed { get; set; }
        string EnrollmentNotes { get; set; }
    }
}