using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CAProxy.AnyGateway;
using CAProxy.AnyGateway.Interfaces;
using CAProxy.AnyGateway.Models;
using CAProxy.AnyGateway.Models.Configuration;
using CAProxy.Common;
using CSS.Common.Logging;
using CSS.PKI;
using Keyfactor.AnyGateway.DigiCertSym.Client;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym
{
    public class DigiCertSymProxy : BaseCAConnector
    {
        private RequestManager _requestManager;

        private IDigiCertSymClient DigiCertSymClient { get; set; }

        private Dictionary<string, ProductModel> Templates { get; set; }


        public override int Revoke(string caRequestId, string hexSerialNumber, uint revocationReason)
        {
            try
            {
                Logger.Trace("Staring Revoke Method");
                var revokeRequest = _requestManager.GetRevokeRequest(revocationReason);

                var revokeResponse =
                    Task.Run(async () =>
                            await DigiCertSymClient.SubmitRevokeCertificateAsync(hexSerialNumber, revokeRequest))
                        .Result;

                Logger.Trace($"Revoke Response JSON: {JsonConvert.SerializeObject(revokeResponse)}");

                var revokeResult = _requestManager.GetRevokeResult(revokeResponse);

                if (revokeResult == Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.FAILED))
                    throw new Exception("Revoke failed");

                return revokeResult;
            }
            catch (Exception e)
            {
                Logger.Error($"Revoke Error Occurred: {e.Message}");
                throw;
            }
        }

        [Obsolete]
        public override void Synchronize(ICertificateDataReader certificateDataReader,
            BlockingCollection<CertificateRecord> blockingBuffer,
            CertificateAuthoritySyncInfo certificateAuthoritySyncInfo, CancellationToken cancelToken,
            string logicalName)
        {
        }

        public override void Synchronize(ICertificateDataReader certificateDataReader,
            BlockingCollection<CAConnectorCertificate> blockingBuffer,
            CertificateAuthoritySyncInfo certificateAuthoritySyncInfo,
            CancellationToken cancelToken)
        {
            try
            {
                //Loop through all the Digicert Profile OIDs that are setup in the config file
                foreach (var productModel in Templates.Values)
                {

                    var pageCounter = 0;
                    var pageSize = 50;
                    var result = DigiCertSymClient.SubmitQueryOrderRequest(_requestManager, productModel, pageCounter);
                    var totalResults = result.certificateCount;
                    var totalPages = (totalResults + pageSize-1)/pageSize;

                    Logger.Trace($"Product Model {productModel} Total Results {totalResults}, Total Pages {totalPages}");

                    if (result.certificateCount > 0)
                    {
                        for (var i = 0; i < totalPages; i++)
                        {
                            //If you need multiple pages make the request again
                            if (pageCounter > 0)
                            {
                                result = DigiCertSymClient.SubmitQueryOrderRequest(_requestManager, productModel,
                                    pageCounter);
                            }

                            XmlSerializer x = new XmlSerializer(result.GetType());
                            TextWriter tw = new StringWriter();
                            x.Serialize(tw, result);
                            Logger.Trace($"Raw Search Cert Soap Response {tw}");

                            foreach (var currentResponseItem in result.certificateList)
                            {
                                try
                                {
                                    Logger.Trace($"Took Certificate ID {currentResponseItem?.serialNumber} from Queue");

                                    if (currentResponseItem != null)
                                    {
                                        var certStatus = _requestManager.MapReturnStatus(currentResponseItem.status);
                                        Logger.Trace($"Certificate Status {certStatus}");
                                        
                                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

                                        //Keyfactor sync only seems to work when there is a valid cert and I can only get Active valid certs from SSLStore
                                        if (certStatus ==
                                            Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.ISSUED) ||
                                            certStatus ==
                                            Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.REVOKED))
                                        {
                                            
                                            blockingBuffer.Add(new CAConnectorCertificate
                                            {
                                                CARequestID =
                                                    $"{currentResponseItem.serialNumber}",
                                                Certificate = Convert.ToBase64String(currentResponseItem.certificate),
                                                SubmissionDate = dateTime.AddSeconds(currentResponseItem.validFrom)
                                                    .ToLocalTime(),
                                                Status = certStatus,
                                                ProductID = $"{currentResponseItem.profileOID}"
                                            }, cancelToken);
                                        }
                                    }
                                }
                                catch (OperationCanceledException e)
                                {
                                    Logger.Error($"Synchronize was canceled. {e.Message}");
                                    break;
                                }
                            }
                            pageCounter += pageSize;
                        }
                    }
                }
            }
            catch (AggregateException aggEx)
            {
                Logger.Error("Digicert mPKI Synchronize Task failed!");
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                // ReSharper disable once PossibleIntendedRethrow
                throw aggEx;
            }

            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
        }

        [Obsolete]
        public override EnrollmentResult Enroll(string csr, string subject, Dictionary<string, string[]> san,
            EnrollmentProductInfo productInfo,
            PKIConstants.X509.RequestFormat requestFormat, RequestUtilities.EnrollmentType enrollmentType)
        {
            return null;
        }

        public override EnrollmentResult Enroll(ICertificateDataReader certificateDataReader, string csr,
            string subject, Dictionary<string, string[]> san, EnrollmentProductInfo productInfo,
            PKIConstants.X509.RequestFormat requestFormat, RequestUtilities.EnrollmentType enrollmentType)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);

            EnrollmentRequest enrollmentRequest;
            EnrollmentRequest renewRequest;

            try
            {
                switch (enrollmentType)
                {
                    case RequestUtilities.EnrollmentType.New:
                        Logger.Trace("Entering New Enrollment");
                        //If they renewed an expired cert it gets here and this will not be supported
                        IEnrollmentResponse enrollmentResponse;

                        enrollmentRequest = _requestManager.GetEnrollmentRequest(productInfo, csr, san);
                        Logger.Trace($"Enrollment Request JSON: {JsonConvert.SerializeObject(enrollmentRequest)}");
                        enrollmentResponse =
                            Task.Run(async () => await DigiCertSymClient.SubmitEnrollmentAsync(enrollmentRequest))
                                .Result;

                        if (enrollmentResponse?.Result == null)
                            return new EnrollmentResult
                            {
                                Status = 30, //failure
                                StatusMessage =
                                    $"Enrollment Failed: {_requestManager.FlattenErrors(enrollmentResponse?.RegistrationError.Errors)}"
                            };


                        Logger.Trace($"Enrollment Response JSON: {JsonConvert.SerializeObject(enrollmentResponse)}");

                        Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                        return _requestManager.GetEnrollmentResult(enrollmentResponse);
                    case RequestUtilities.EnrollmentType.Renew:
                    case RequestUtilities.EnrollmentType.Reissue:
                        Logger.Trace("Entering Renew Enrollment");
                        Logger.Trace("Checking To Make sure it is not one click renew (not supported)");
                        //KeyFactor needs a better way to detect one click renewals, some flag or something
                        if (productInfo.ProductParameters.Count > 7)
                        {
                            var priorCertSn = productInfo.ProductParameters["PriorCertSN"];
                            Logger.Trace($"Renew Serial Number: {priorCertSn}");
                            renewRequest = _requestManager.GetEnrollmentRequest(productInfo, csr, san);

                            Logger.Trace($"Renewal Request JSON: {JsonConvert.SerializeObject(renewRequest)}");
                            var renewResponse = Task.Run(async () =>
                                    await DigiCertSymClient.SubmitRenewalAsync(priorCertSn, renewRequest))
                                .Result;
                            if (renewResponse?.Result == null)
                                return new EnrollmentResult
                                {
                                    Status = 30, //failure
                                    StatusMessage =
                                        $"Enrollment Failed {_requestManager.FlattenErrors(renewResponse?.RegistrationError.Errors)}"
                                };

                            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                            return _requestManager.GetRenewResponse(renewResponse);
                        }
                        else
                        {
                            return new EnrollmentResult
                            {
                                Status = 30, //failure
                                StatusMessage =
                                "One Click Renew is not available for this integration.  Need to specify validity and seat enrollment params."
                            };
                        }

                }

                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                return null;
            }
            catch (Exception e)
            {
                Logger.Error($"Enrollment Error Occurred: {e.Message}");
                throw;
            }
        }


        public override CAConnectorCertificate GetSingleRecord(string caRequestId)
        {
            try
            {
                Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
                var keyfactorCaId = caRequestId;
                Logger.Trace($"Keyfactor Ca Id: {keyfactorCaId}");
                var certificateResponse =
                    Task.Run(async () => await DigiCertSymClient.SubmitGetCertificateAsync(keyfactorCaId))
                        .Result;

                Logger.Trace($"Single Cert JSON: {JsonConvert.SerializeObject(certificateResponse)}");
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                return new CAConnectorCertificate
                {
                    CARequestID = keyfactorCaId,
                    Certificate = certificateResponse.Result.Certificate,
                    Status = _requestManager.MapReturnStatus(certificateResponse.Result.Status),
                    SubmissionDate = Convert.ToDateTime(certificateResponse.Result.ValidFrom)
                };
            }
            catch (Exception e)
            {
                Logger.Error($"GetSingleRecord Error Occurred: {e.Message}");
                throw;
            }
        }

        public override void Initialize(ICAConnectorConfigProvider configProvider)
        {
            try
            {
                Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
                var config = (CAConfig)configProvider;
                _requestManager = new RequestManager
                {

                    DnsConstantName = configProvider.CAConnectionData["DnsConstantName"].ToString(),
                    UpnConstantName = configProvider.CAConnectionData["UpnConstantName"].ToString(),
                    IpConstantName = configProvider.CAConnectionData["IpConstantName"].ToString(),
                    EmailConstantName = configProvider.CAConnectionData["EmailConstantName"].ToString()
                };
                DigiCertSymClient = new DigiCertSymClient(config);
                Templates = config.Config.Templates;
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
            }
            catch (Exception e)
            {
                Logger.Error($"Initialize Error Occurred: {e.Message}");
                throw;
            }
        }

        public override void Ping()
        {
        }

        public override void ValidateCAConnectionInfo(Dictionary<string, object> connectionInfo)
        {
        }

        public override void ValidateProductInfo(EnrollmentProductInfo productInfo,
            Dictionary<string, object> connectionInfo)
        {
        }
    }
}
