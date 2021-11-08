using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CAProxy.AnyGateway.Interfaces;
using CSS.Common.Logging;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.Exceptions;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client
{
    public sealed class DigiCertSymClient : LoggingClientBase, IDigiCertSymClient
    {
        public DigiCertSymClient(ICAConnectorConfigProvider config)
        {
            if (config.CAConnectionData.ContainsKey(Constants.DigiCertSymApiKey))
            {
                try
                {
                    BaseUrl = new Uri(config.CAConnectionData[Constants.DigiCertSymUrl].ToString());
                    ApiKey = config.CAConnectionData[Constants.DigiCertSymApiKey].ToString();
                    SeatList = config.CAConnectionData[Constants.SeatList].ToString();
                    RestClient = ConfigureRestClient();
                }
                catch (Exception e)
                {
                    Logger.Error($"DigiCertSymClient Constructor Error Occurred: {e.Message}");
                    throw;
                }
            }
        }

        private Uri BaseUrl { get; }
        private HttpClient RestClient { get; }
        private string ApiKey { get; }
        private string SeatList { get; }
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

        public async Task SubmitQueryOrderRequestAsync(BlockingCollection<ICertificateDetails> bc, CancellationToken ct,
    RequestManager requestManager)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            try
            {
                var itemsProcessed = 0;
                var pageCounter = 1;
                var isComplete = false;
                var retryCount = 0;

                foreach (var seat in SeatList.Split(','))
                {
                    Logger.Trace($"Processing SeatId {seat}");
                    pageCounter = 1;
                    do
                    {
                        var queryOrderRequest =
                            requestManager.GetSearchCertificatesRequest(pageCounter, seat);
                        var batchItemsProcessed = 0;
                        using (var resp = await RestClient.PostAsync("/mpki/api/v1/searchcert", new StringContent(
                            JsonConvert.SerializeObject(queryOrderRequest), Encoding.ASCII, "application/json"), ct))
                        {

                        if (!resp.IsSuccessStatusCode)
                        {
                            var responseMessage = resp.Content.ReadAsStringAsync().Result;
                            Logger.Trace($"Raw error response {responseMessage}");

                            //igngore missing Certificate in search 404 errors
                            if (!responseMessage.Contains("entity_not_found"))
                            {
                                Logger.Error(
                                    $"Failed Request to Digicert mPKI. Retrying request. Status Code {resp.StatusCode} | Message: {responseMessage}");
                                retryCount++;
                                if (retryCount > 5)
                                    throw new RetryCountExceededException(
                                        $"5 consecutive failures to {resp.RequestMessage.RequestUri}");
                            }
                            break; //Seat has no certs move on to the next seat
                        }

                            var response = JsonConvert.DeserializeObject<CertificateSearchResponse>(
                                await resp.Content.ReadAsStringAsync());

                            var batchResponse = response.Certificates;
                            var batchCount = batchResponse.Count;

                            Logger.Trace($"Processing {batchCount} items in batch");
                            do
                            {
                                var r = batchResponse[batchItemsProcessed];
                                if (bc.TryAdd(r, 10, ct))
                                {
                                    Logger.Trace($"Added Certificate ID {r.SerialNumber} to Queue for processing");
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
                        }

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