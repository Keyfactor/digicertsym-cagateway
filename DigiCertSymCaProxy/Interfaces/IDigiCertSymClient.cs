// Copyright 2023 Keyfactor                                                   
// Licensed under the Apache License, Version 2.0 (the "License"); you may    
// not use this file except in compliance with the License.  You may obtain a 
// copy of the License at http://www.apache.org/licenses/LICENSE-2.0.  Unless 
// required by applicable law or agreed to in writing, software distributed   
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES   
// OR CONDITIONS OF ANY KIND, either express or implied. See the License for  
// thespecific language governing permissions and limitations under the       
// License. 
﻿using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CAProxy.AnyGateway.Models.Configuration;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.DigicertMPKISOAP;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IDigiCertSymClient
    {
        Task<EnrollmentResponse> SubmitEnrollmentAsync(
            EnrollmentRequest enrollmentRequest);

        Task<EnrollmentResponse> SubmitRenewalAsync(string serialNumber,
            EnrollmentRequest renewalRequest);

        Task<RevokeResponse> SubmitRevokeCertificateAsync(string serialNumber, RevokeRequest req);

        Task<GetCertificateResponse> SubmitGetCertificateAsync(string serialNumber);

        SearchCertificateResponseType SubmitQueryOrderRequest(
            RequestManager requestManager, ProductModel template, int pageCounter);
    }
}