using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CAProxy.AnyGateway;
using CAProxy.AnyGateway.Models.Configuration;
using CSS.Common.Logging;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.DigicertMPKISOAP;
using Keyfactor.AnyGateway.DigiCertSym.Exceptions;
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
                    Templates = config.Config.Templates;
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
        private Dictionary<string, ProductModel> Templates { get; }
        private string EndPointAddress { get; }
        private string ClientCertificateLocation { get; }
        private string ClientCertificatePassword { get; }
        private int PageSize { get; } = 50;


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
                    var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                    if (resp.StatusCode == HttpStatusCode.BadRequest) //DigiCert Sends Errors back in 400 Json Response
                    {
                        var errorResponse =
                            JsonConvert.DeserializeObject<ErrorList>(await resp.Content.ReadAsStringAsync(),
                                settings);
                        response = new EnrollmentResponse {RegistrationError = errorResponse, Result = null};
                        return response;
                    }

                    var registrationResponse =
                        JsonConvert.DeserializeObject<EnrollmentSuccessResponse>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    response = new EnrollmentResponse {RegistrationError = null, Result = registrationResponse};
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
                    var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                    if (resp.StatusCode == HttpStatusCode.BadRequest) //DigiCert Sends Errors back in 400 Json Response
                    {
                        var errorResponse =
                            JsonConvert.DeserializeObject<ErrorList>(await resp.Content.ReadAsStringAsync(),
                                settings);
                        response = new EnrollmentResponse {RegistrationError = errorResponse, Result = null};
                        return response;
                    }

                    var registrationResponse =
                        JsonConvert.DeserializeObject<EnrollmentSuccessResponse>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    response = new EnrollmentResponse {RegistrationError = null, Result = registrationResponse};
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
                    var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
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
                    response = new RevokeResponse {RegistrationError = null, Result = getRevokeResponse};
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

                    var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                    GetCertificateResponse response;
                    if (resp.StatusCode == HttpStatusCode.BadRequest) //DigiCert Sends Errors back in 400 Json Response
                    {
                        var errorResponse =
                            JsonConvert.DeserializeObject<ErrorList>(await resp.Content.ReadAsStringAsync(),
                                settings);
                        response = new GetCertificateResponse {CertificateError = errorResponse, Result = null};
                        return response;
                    }

                    var certificateResponse =
                        JsonConvert.DeserializeObject<CertificateDetails>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    response = new GetCertificateResponse {CertificateError = null, Result = certificateResponse};
                    return response;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"SubmitGetCertificateAsync Error Occurred {e.Message}");
                throw;
            }
        }

        public async Task SubmitQueryOrderRequestAsync(BlockingCollection<CertificateSearchResultType> bc,
            CancellationToken ct,
            RequestManager requestManager)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            try
            {
                var itemsProcessed = 0;
                var isComplete = false;

                foreach (var template in Templates.Values)
                {
                    Logger.Trace($"Processing Template {template.ProductID}");
                    var pageCounter = 0;
                    do
                    {
                        var queryOrderRequest =
                            requestManager.GetSearchCertificatesRequest(pageCounter, template.ProductID);
                        var batchItemsProcessed = 0;

                        var bind = new BasicHttpsBinding();
                        bind.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                        var ep = new EndpointAddress(EndPointAddress);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                        var client = new CertificateManagementOperationsClient(bind, ep);
                        var cert = new X509Certificate2(ClientCertificateLocation, ClientCertificatePassword);
                        if (client.ClientCredentials != null)
                            client.ClientCredentials.ClientCertificate.Certificate = cert;


                        var resp = await client.searchCertificateAsync(queryOrderRequest);

                        if (resp.searchCertificateResponse1.certificateCount == 0)
                            break; //Profile has no certs move on to the next Profile

                        var batchResponse = resp.searchCertificateResponse1.certificateList;
                        var batchCount = batchResponse.Length;

                        Logger.Trace($"Processing {batchCount} items in batch");
                        do
                        {
                            var r = batchResponse[batchItemsProcessed];
                            if (bc.TryAdd(r, 10, ct))
                            {
                                Logger.Trace($"Added Certificate ID {r.serialNumber} to Queue for processing");
                                batchItemsProcessed++;
                                itemsProcessed++;
                                Logger.Trace($"Processed {batchItemsProcessed} of {batchCount}");
                                Logger.Trace($"Total Items Processed: {itemsProcessed}");
                            }
                            else
                            {
                                Logger.Trace($"Adding {r} blocked. Retry");
                            }
                        } while (batchItemsProcessed < batchCount); //batch loop


                        //assume that if we process less records than requested that we have reached the end of the certificate list
                        if (batchItemsProcessed < PageSize)
                            isComplete = true;
                        pageCounter = pageCounter + PageSize;
                    } while (!isComplete); //page loop
                }

                bc.CompleteAdding();
            }
            catch (OperationCanceledException cancelEx)
            {
                Logger.Warn($"Synchronize method was cancelled. Message: {cancelEx.Message}");
                bc.CompleteAdding();
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                // ReSharper disable once PossibleIntendedRethrow
                throw cancelEx;
            }
            catch (RetryCountExceededException retryEx)
            {
                Logger.Error($"Retries Failed: {retryEx.Message}");
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
            }
            catch (HttpRequestException ex)
            {
                Logger.Error($"HttpRequest Failed: {ex.Message}");
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
            }

            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
        }

        private HttpClient ConfigureRestClient()
        {
            try
            {
                var clientHandler = new WebRequestHandler();
                var returnClient = new HttpClient(clientHandler, true) {BaseAddress = BaseUrl};
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