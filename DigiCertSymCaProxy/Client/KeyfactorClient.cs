using System;
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

        private HttpClient RestClient { get; }

        public async Task<List<KeyfactorCertificate>> SubmitGetKeyfactorCertAsync(string thumbprintFilter)
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
    }
}