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
            using (var resp = await RestClient.PostAsync("/mpki/api/v1/certificate", new StringContent(
                JsonConvert.SerializeObject(enrollmentRequest), Encoding.ASCII, "application/json")))
            {
                EnrollmentResponse response;
                Logger.Trace(JsonConvert.SerializeObject(enrollmentRequest));
                var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                if (resp.StatusCode == HttpStatusCode.BadRequest) //Digicert Sends Errors back in 400 Json Response
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

        public async Task<EnrollmentResponse> SubmitRenewalAsync(string serialNumber,
            EnrollmentRequest renewalRequest)
        {
            using (var resp = await RestClient.PostAsync($"/mpki/api/v1/certificate/{serialNumber}/renew",
                new StringContent(
                    JsonConvert.SerializeObject(renewalRequest), Encoding.ASCII, "application/json")))
            {
                EnrollmentResponse response;
                Logger.Trace(JsonConvert.SerializeObject(renewalRequest));
                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                if (resp.StatusCode == HttpStatusCode.BadRequest) //Digicert Sends Errors back in 400 Json Response
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

        public async Task<RevokeResponse> SubmitRevokeCertificateAsync(string serialNumber,RevokeRequest revokeRequest)
        {
            RevokeResponse response=new RevokeResponse();

            using (var resp = await RestClient.PutAsync($"/mpki/api/v1/certificate/{serialNumber}/revoke",
                new StringContent(
                    JsonConvert.SerializeObject(revokeRequest), Encoding.ASCII, "application/json")))
            {
                var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                if (resp.StatusCode == HttpStatusCode.BadRequest) //Digicert Sends Errors back in 400 Json Response
                {
                    var errorResponse =
                        JsonConvert.DeserializeObject<ErrorList>(await resp.Content.ReadAsStringAsync(),
                            settings);
                    response.RegistrationError = errorResponse;
                    response.Result = null;
                    return response;
                }

                var getRevokeResponse = (await resp.Content.ReadAsStringAsync());
                response = new RevokeResponse { RegistrationError = null, Result = getRevokeResponse};
                return response;
            }
        }

        public async Task<GetCertificateResponse> SubmitGetCertificateAsync(string serialNumber)
        {
            GetCertificateResponse response;
            using (var resp = await RestClient.GetAsync($"/mpki/api/v1/certificate/{serialNumber}"))
            {
                Logger.Trace(JsonConvert.SerializeObject(resp));

                var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                if (resp.StatusCode == HttpStatusCode.BadRequest) //Digicert Sends Errors back in 400 Json Response
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