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
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CAProxy.AnyGateway.Interfaces;
using CSS.Common.Logging;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client
{
    public sealed class KeyfactorClient : LoggingClientBase, IKeyfactorClient
    {
        public KeyfactorClient(ICAConnectorConfigProvider configProvider)
        {
            try
            {
                var keyfactorBaseUrl = new Uri(configProvider.CAConnectionData[Constants.KeyfactorApiUrl].ToString());
                var keyfactorAuth = configProvider.CAConnectionData[Constants.KeyfactorApiUserId] + ":" +
                                    configProvider.CAConnectionData[Constants.KeyfactorApiPassword];
                var plainTextBytes = Encoding.UTF8.GetBytes(keyfactorAuth);

                var clientHandler = new WebRequestHandler();
                RestClient = new HttpClient(clientHandler, true) {BaseAddress = keyfactorBaseUrl};
                RestClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                RestClient.DefaultRequestHeaders.Add("x-keyfactor-requested-with", "APIClient");
                RestClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(plainTextBytes));
            }
            catch (Exception e)
            {
                Logger.Error($"Error In KeyfactorClient {e.Message}");
                throw;
            }
        }

        private HttpClient RestClient { get; }

        public async Task<List<KeyfactorCertificate>> SubmitGetKeyfactorCertAsync(string thumbprintFilter)
        {
            try
            {
                using (var resp =
                    await RestClient.GetAsync(
                        $"/KeyfactorApi/Certificates?pq.queryString=Thumbprint%20-eq%20%22{thumbprintFilter}%22"))
                {
                    resp.EnsureSuccessStatusCode();
                    var keyfactorCertificateResponse =
                        JsonConvert.DeserializeObject<List<KeyfactorCertificate>>(await resp.Content.ReadAsStringAsync());
                    return keyfactorCertificateResponse;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error In SubmitGetKeyfactorCertAsync {e.Message}");
                throw;
            }
        }
    }
}