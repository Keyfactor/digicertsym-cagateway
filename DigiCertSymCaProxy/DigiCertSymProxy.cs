using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CAProxy.AnyGateway;
using CAProxy.AnyGateway.Interfaces;
using CAProxy.AnyGateway.Models;
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
        private readonly RequestManager _requestManager;

        public DigiCertSymProxy()
        {
            _requestManager = new RequestManager();
        }

        private IDigiCertSymClient DigiCertSymClient { get; set; }


        public override int Revoke(string caRequestId, string hexSerialNumber, uint revocationReason)
        {
            Logger.Trace("Staring Revoke Method");
            var revokeRequest= _requestManager.GetRevokeRequest(revocationReason);

            var revokeResponse =
                Task.Run(async () =>
                        await DigiCertSymClient.SubmitRevokeCertificateAsync(hexSerialNumber,revokeRequest))
                    .Result;

            Logger.Trace($"Revoke Response JSON: {JsonConvert.SerializeObject(revokeResponse)}");

            var revokeResult = _requestManager.GetRevokeResult(revokeResponse);

            if (revokeResult == Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.FAILED))
                throw new Exception("Revoke failed");

            return revokeResult;
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
                            StatusMessage = $"Enrollment Failed: {_requestManager.FlattenErrors(enrollmentResponse?.RegistrationError.errors)}"
                        };


                    Logger.Trace($"Enrollment Response JSON: {JsonConvert.SerializeObject(enrollmentResponse)}");

                    Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                    return _requestManager.GetEnrollmentResult(enrollmentResponse);
                case RequestUtilities.EnrollmentType.Renew:
                case RequestUtilities.EnrollmentType.Reissue:
                    Logger.Trace("Entering Renew Enrollment");

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
                                $"Enrollment Failed {_requestManager.FlattenErrors(renewResponse?.RegistrationError.errors)}"
                        };

                    Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                    return _requestManager.GetRenewResponse(renewResponse);
            }

            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
            return null;
        }


        public override CAConnectorCertificate GetSingleRecord(string caRequestId)
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

        public override void Initialize(ICAConnectorConfigProvider configProvider)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            DigiCertSymClient = new DigiCertSymClient(configProvider);
            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
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