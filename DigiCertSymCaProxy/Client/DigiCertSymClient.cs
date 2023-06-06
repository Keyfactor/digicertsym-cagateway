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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CAProxy.AnyGateway;
using CAProxy.AnyGateway.Models.Configuration;
using CSS.Common.Logging;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.DigicertMPKISOAP;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client
{
    public sealed class DigiCertSymClient : LoggingClientBase, IDigiCertSymClient
    {
        public DigiCertSymClient(CAConfig config)
        {
            if (config.Config.CAConnection.ContainsKey(Constants.DigiCertSymApiKey))
                try
                {
                    BaseUrl = new Uri(config.Config.CAConnection[Constants.DigiCertSymUrl].ToString());
                    ApiKey = config.Config.CAConnection[Constants.DigiCertSymApiKey].ToString();
                    ClientCertificateLocation =
                        config.Config.CAConnection[Constants.ClientCertificateLocation].ToString();
                    ClientCertificatePassword =
                        config.Config.CAConnection[Constants.ClientCertificatePassword].ToString();
                    EndPointAddress = config.Config.CAConnection[Constants.EndpointAddress].ToString();
                    RestClient = ConfigureRestClient();
                }
                catch (Exception e)
                {
                    Logger.Error($"DigiCertSymClient Constructor Error Occurred: {e.Message}");
                    throw;
                }
        }

        private Uri BaseUrl { get; }
        private HttpClient RestClient { get; }
        private string ApiKey { get; }
        private string EndPointAddress { get; }
        private string ClientCertificateLocation { get; }
        private string ClientCertificatePassword { get; }


        public async Task<EnrollmentResponse> SubmitEnrollmentAsync(
            EnrollmentRequest enrollmentRequest)
        {
            try
            {
                using (var resp = await RestClient.PostAsync("/mpki/api/v1/certificate", new StringContent(
                    JsonConvert.SerializeObject(enrollmentRequest), Encoding.ASCII, "application/json")))
                {
                    EnrollmentResponse response;
                    Logger.Trace(JsonConvert.SerializeObject(enrollmentRequest));
                    var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    if (resp.StatusCode == HttpStatusCode.BadRequest) //DigiCert Sends Errors back in 400 Json Response
                    {
                        var errorResponse =
                            JsonConvert.DeserializeObject<ErrorList>(await resp.Content.ReadAsStringAsync(),
                                settings);
                        response = new EnrollmentResponse { RegistrationError = errorResponse, Result = null };
                        return response;
                    }

                    var registrationResponse =
                        JsonConvert.DeserializeObject<EnrollmentSuccessResponse>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    response = new EnrollmentResponse { RegistrationError = null, Result = registrationResponse };
                    return response;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"SubmitEnrollmentAsync Error Occurred {e.Message}");
                throw;
            }
        }

        public async Task<EnrollmentResponse> SubmitRenewalAsync(string serialNumber,
            EnrollmentRequest renewalRequest)
        {
            try
            {
                using (var resp = await RestClient.PostAsync($"/mpki/api/v1/certificate/{serialNumber}/renew",
                    new StringContent(
                        JsonConvert.SerializeObject(renewalRequest), Encoding.ASCII, "application/json")))
                {
                    EnrollmentResponse response;
                    Logger.Trace(JsonConvert.SerializeObject(renewalRequest));
                    var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    if (resp.StatusCode == HttpStatusCode.BadRequest) //DigiCert Sends Errors back in 400 Json Response
                    {
                        var errorResponse =
                            JsonConvert.DeserializeObject<ErrorList>(await resp.Content.ReadAsStringAsync(),
                                settings);
                        response = new EnrollmentResponse { RegistrationError = errorResponse, Result = null };
                        return response;
                    }

                    var registrationResponse =
                        JsonConvert.DeserializeObject<EnrollmentSuccessResponse>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    response = new EnrollmentResponse { RegistrationError = null, Result = registrationResponse };
                    return response;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"SubmitRenewalAsync Error Occurred {e.Message}");
                throw;
            }
        }

        public async Task<RevokeResponse> SubmitRevokeCertificateAsync(string serialNumber, RevokeRequest revokeRequest)
        {
            try
            {
                var response = new RevokeResponse();

                using (var resp = await RestClient.PutAsync($"/mpki/api/v1/certificate/{serialNumber}/revoke",
                    new StringContent(
                        JsonConvert.SerializeObject(revokeRequest), Encoding.ASCII, "application/json")))
                {
                    var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    if (resp.StatusCode == HttpStatusCode.BadRequest) //DigiCert Sends Errors back in 400 Json Response
                    {
                        var errorResponse =
                            JsonConvert.DeserializeObject<ErrorList>(await resp.Content.ReadAsStringAsync(),
                                settings);
                        response.RegistrationError = errorResponse;
                        response.Result = null;
                        return response;
                    }

                    var getRevokeResponse = await resp.Content.ReadAsStringAsync();
                    response = new RevokeResponse { RegistrationError = null, Result = getRevokeResponse };
                    return response;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"SubmitRevokeCertificateAsync Error Occurred {e.Message}");
                throw;
            }
        }

        public async Task<GetCertificateResponse> SubmitGetCertificateAsync(string serialNumber)
        {
            try
            {
                using (var resp = await RestClient.GetAsync($"/mpki/api/v1/certificate/{serialNumber}"))
                {
                    Logger.Trace(JsonConvert.SerializeObject(resp));

                    var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    GetCertificateResponse response;
                    if (resp.StatusCode == HttpStatusCode.BadRequest) //DigiCert Sends Errors back in 400 Json Response
                    {
                        var errorResponse =
                            JsonConvert.DeserializeObject<ErrorList>(await resp.Content.ReadAsStringAsync(),
                                settings);
                        response = new GetCertificateResponse { CertificateError = errorResponse, Result = null };
                        return response;
                    }

                    var certificateResponse =
                        JsonConvert.DeserializeObject<CertificateDetails>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    response = new GetCertificateResponse { CertificateError = null, Result = certificateResponse };
                    return response;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"SubmitGetCertificateAsync Error Occurred {e.Message}");
                throw;
            }
        }

        public SearchCertificateResponseType SubmitQueryOrderRequest(
            RequestManager requestManager, ProductModel template,int pageCounter)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            try
            {
                Logger.Trace($"Processing Template {template.ProductID}");

                var queryOrderRequest =
                    requestManager.GetSearchCertificatesRequest(pageCounter, template.ProductID);
                XmlSerializer x = new XmlSerializer(queryOrderRequest.GetType());
                TextWriter tw = new StringWriter();
                x.Serialize(tw, queryOrderRequest);
                Logger.Trace($"Raw Search Cert Soap Request {tw}");

                var bind = new BasicHttpsBinding {MaxReceivedMessageSize = 2147483647};
                bind.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                var ep = new EndpointAddress(EndPointAddress);
                var client = new CertificateManagementOperationsClient(bind, ep);
                var cert = new X509Certificate2(ClientCertificateLocation, ClientCertificatePassword);
                if (client.ClientCredentials != null)
                    client.ClientCredentials.ClientCertificate.Certificate = cert;

                var resp = client.searchCertificate(queryOrderRequest);

                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);

                return resp;
            }
            catch (Exception e)
            {
                Logger.Error($"CertificateSearchResultType Error Occurred {e.Message}");
                throw;
            }
        }

        private HttpClient ConfigureRestClient()
        {
            try
            {
                var clientHandler = new WebRequestHandler();
                var returnClient = new HttpClient(clientHandler, true) { BaseAddress = BaseUrl };
                returnClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                returnClient.DefaultRequestHeaders.Add("x-api-key", ApiKey);
                return returnClient;
            }
            catch (Exception e)
            {
                Logger.Error($"ConfigureRestClient Error Occurred {e.Message}");
                throw;
            }
        }
    }
}