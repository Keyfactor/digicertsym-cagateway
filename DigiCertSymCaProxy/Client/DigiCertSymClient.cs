using System;
using System.Collections.Generic;
using System.Net;
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
    public sealed class DigiCertSymClient : LoggingClientBase, IDigiCertSymClient
    {
        public DigiCertSymClient(ICAConnectorConfigProvider config)
        {
            if (config.CAConnectionData.ContainsKey(Constants.DigiCertSymApiKey))
            {
                BaseUrl = new Uri(config.CAConnectionData[Constants.DigiCertSymUrl].ToString());
                ApiKey = config.CAConnectionData[Constants.DigiCertSymApiKey].ToString();
                RestClient = ConfigureRestClient();
            }
        }

        private Uri BaseUrl { get; }
        private HttpClient RestClient { get; }
        private string ApiKey { get; }


        public async Task<EnrollmentResponse> SubmitEnrollmentAsync(
            EnrollmentRequest enrollmentRequest)
        {
            using (var resp = await RestClient.PostAsync("/api/v1/certificate", new StringContent(
                JsonConvert.SerializeObject(enrollmentRequest), Encoding.ASCII, "application/json")))
            {
                Logger.Trace(JsonConvert.SerializeObject(enrollmentRequest));
                var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                if (resp.StatusCode == HttpStatusCode.BadRequest) //Digicert Sends Errors back in 400 Json Response
                {
                    var errorResponse =
                        JsonConvert.DeserializeObject<List<ErrorResponse>>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    var response = new EnrollmentResponse {RegistrationError = errorResponse, Result = null};
                    return response;
                }

                var registrationResponse =
                    JsonConvert.DeserializeObject<EnrollmentResponse>(await resp.Content.ReadAsStringAsync(),
                        settings);
                return registrationResponse;
            }
        }

        public async Task<EnrollmentResponse> SubmitRenewalAsync(string serialNumber,
            EnrollmentRequest renewalRequest)
        {
            using (var resp = await RestClient.PostAsync($"/api/v1/certificate/{serialNumber}/renew", new StringContent(
                JsonConvert.SerializeObject(renewalRequest), Encoding.ASCII, "application/json")))
            {
                Logger.Trace(JsonConvert.SerializeObject(renewalRequest));

                var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                if (resp.StatusCode == HttpStatusCode.BadRequest) //Digicert Sends Errors back in 400 Json Response
                {
                    var errorResponse =
                        JsonConvert.DeserializeObject<List<ErrorResponse>>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    var response = new EnrollmentResponse { RegistrationError = errorResponse, Result = null };
                    return response;
                }

                var registrationResponse =
                    JsonConvert.DeserializeObject<EnrollmentResponse>(await resp.Content.ReadAsStringAsync(),
                        settings);
                return registrationResponse;
            }
        }

        public async Task<RevokeResponse> SubmitRevokeCertificateAsync(string serialNumber)
        {
            using (var resp = await RestClient.PutAsync($"/api/v1/certificate/{serialNumber}/revoke", new StringContent("")))
            {
                var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                if (resp.StatusCode == HttpStatusCode.BadRequest) //Digicert Sends Errors back in 400 Json Response
                {
                    var errorResponse =
                        JsonConvert.DeserializeObject<List<ErrorResponse>>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    var response = new RevokeResponse();
                    response.RegistrationError = errorResponse;
                    response.Result = null;
                    return response;
                }

                var getRevokeResponse =
                    JsonConvert.DeserializeObject<RevokeResponse>(await resp.Content.ReadAsStringAsync());
                return getRevokeResponse;
            }
        }

        public async Task<GetCertificateResponse> SubmitGetCertificateAsync(string serialNumber)
        {
            using (var resp = await RestClient.GetAsync($"/api/v1/certificate/{serialNumber}"))
            {
                Logger.Trace(JsonConvert.SerializeObject(resp));

                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                if (resp.StatusCode == HttpStatusCode.BadRequest) //Digicert Sends Errors back in 400 Json Response
                {
                    var errorResponse =
                        JsonConvert.DeserializeObject<List<ErrorResponse>>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    var response = new GetCertificateResponse() { CertificateError = errorResponse, Result = null };
                    return response;
                }

                var certificateResponse =
                    JsonConvert.DeserializeObject<GetCertificateResponse>(await resp.Content.ReadAsStringAsync(),
                        settings);
                return certificateResponse;
            }
        }

        private HttpClient ConfigureRestClient()
        {
            var clientHandler = new WebRequestHandler();
            var returnClient = new HttpClient(clientHandler, true) {BaseAddress = BaseUrl};
            returnClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            returnClient.DefaultRequestHeaders.Add("x-api-key", ApiKey);
            return returnClient;
        }
    }
}