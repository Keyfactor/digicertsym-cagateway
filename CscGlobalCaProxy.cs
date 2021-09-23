﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CAProxy.AnyGateway;
using CAProxy.AnyGateway.Interfaces;
using CAProxy.AnyGateway.Models;
using CAProxy.Common;
using CSS.Common;
using CSS.Common.Logging;
using CSS.PKI;
using Keyfactor.AnyGateway.CscGlobal.Client;
using Keyfactor.AnyGateway.CscGlobal.Client.Models;
using Keyfactor.AnyGateway.CscGlobal.Interfaces;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.CscGlobal
{
    public class CscGlobalCaProxy : BaseCAConnector
    {
        private readonly RequestManager _requestManager;

        public CscGlobalCaProxy()
        {
            _requestManager = new RequestManager();
        }

        private IKeyfactorClient KeyfactorClient { get; set; }

        private ICscGlobalClient CscGlobalClient { get; set; }
        public bool EnableTemplateSync { get; set; }
        private string CscGlobalEmail { get; set; }
        private string SmtpEmailHost { get; set; }
        private string FromEmailAddress { get; set; }
        private string EmailUserId { get; set; }
        private string EmailPassword { get; set; }
        private int EmailPort { get; set; }


        public override int Revoke(string caRequestId, string hexSerialNumber, uint revocationReason)
        {
            Logger.Trace("Staring Revoke Method");
            var revokeResponse =
                Task.Run(async () =>
                        await CscGlobalClient.SubmitRevokeCertificateAsync(caRequestId.Substring(0, 36)))
                    .Result; //todo fix to use pipe delimiter

            Logger.Trace($"Revoke Response JSON: {JsonConvert.SerializeObject(revokeResponse)}");
            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);

            var revokeResult = _requestManager.GetRevokeResult(revokeResponse);

            if (revokeResult == Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.FAILED))
            {
                throw new Exception("Revoke failed");
                return -1;
            }

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
            Logger.Trace($"Full Sync? {certificateAuthoritySyncInfo.DoFullSync}");
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            try
            {
                var certs = new BlockingCollection<ICertificateResponse>(100);
                CscGlobalClient.SubmitCertificateListRequestAsync(certs, cancelToken);

                foreach (var currentResponseItem in certs.GetConsumingEnumerable(cancelToken))
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        Logger.Error("Synchronize was canceled.");
                        break;
                    }

                    try
                    {
                        Logger.Trace($"Took Certificate ID {currentResponseItem?.Uuid} from Queue");
                        var certStatus = _requestManager.MapReturnStatus(currentResponseItem?.Status);

                        //Keyfactor sync only seems to work when there is a valid cert and I can only get Active valid certs from Csc Global
                        if (certStatus == Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.ISSUED) ||
                            certStatus == Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.REVOKED))
                        {
                            //One click renewal/reissue won't work for this implementation so there is an option to disable it by not syncing back template
                            var productId = "CscGlobal";
                            if (EnableTemplateSync) productId = currentResponseItem?.CertificateType;

                            var fileContent =
                                Encoding.ASCII.GetString(
                                    Convert.FromBase64String(currentResponseItem?.Certificate ?? string.Empty));

                            Logger.Trace($"Certificate Content: {fileContent}");

                            if (fileContent.Length > 0)
                            {
                                var certData = fileContent.Replace("\r\n", string.Empty);
                                var splitCerts =
                                    certData.Split(new[] { "-----END CERTIFICATE-----", "-----BEGIN CERTIFICATE-----" },
                                        StringSplitOptions.RemoveEmptyEntries);
                                foreach (var cert in splitCerts)
                                    if (!cert.Contains(".crt"))
                                    {
                                        Logger.Trace($"Split Cert Value: {cert}");

                                        var currentCert = new X509Certificate2(Encoding.ASCII.GetBytes(cert));
                                        if (!currentCert.Subject.Contains("AAA Certificate Services") &&
                                            !currentCert.Subject.Contains("USERTrust RSA Certification Authority") &&
                                            !currentCert.Subject.Contains("Trusted Secure Certificate Authority 5") &&
                                            !currentCert.Subject.Contains("AddTrust External CA Root") &&
                                            !currentCert.Subject.Contains("Trusted Secure Certificate Authority DV"))
                                        {
                                            //CSC Specific workflow with Service Now
                                            var keyfactorCertificateResponse = Task.Run(async () =>
                                                await KeyfactorClient.SubmitGetKeyfactorCertAsync(
                                                    currentCert.Thumbprint)).Result;

                                            Logger.Trace($"Get Certs Response: {keyfactorCertificateResponse}");

                                            if (keyfactorCertificateResponse.Count == 0 && currentResponseItem?.Status == "ACTIVE")
                                            {
                                                var bodyLines = new List<string>
                                                {
                                                    $"PO Number: {currentResponseItem?.CustomFields.FirstOrDefault(s=>s.Name=="Purchase Order Number")?.Value}",
                                                    $"Thumbprint: {currentCert.Thumbprint}"
                                                };
                                                var subject =
                                                    $"Keyfactor PO {currentResponseItem?.CustomFields.FirstOrDefault(s => s.Name == "Purchase Order Number")?.Value} Certificate is Ready for Download";
                                                Logger.Trace($"Sending Mail smtp host={SmtpEmailHost} From Email={FromEmailAddress} To Email={CscGlobalEmail} Subject={subject} EmailUserId={EmailUserId} EmailPassword={EmailPassword} Email Port={EmailPort}");
                                                _requestManager.SendEmail(SmtpEmailHost, FromEmailAddress,
                                                    CscGlobalEmail, subject, EmailUserId, EmailPassword, EmailPort, bodyLines);
                                                Logger.Trace($"Email Sent");
                                            }

                                            //Now add the certificate to Keyfactor
                                            blockingBuffer.Add(new CAConnectorCertificate
                                            {
                                                CARequestID = $"{currentResponseItem?.Uuid}-{currentCert.SerialNumber}",
                                                Certificate = cert,
                                                SubmissionDate = currentResponseItem?.OrderDate == null
                                                    ? Convert.ToDateTime(currentCert.NotBefore)
                                                    : Convert.ToDateTime(currentResponseItem.OrderDate),
                                                Status = certStatus,
                                                ProductID = productId
                                            }, cancelToken);
                                        }
                                    }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Logger.Error("Synchronize was canceled.");
                        break;
                    }
                }
            }
            catch (AggregateException aggEx)
            {
                Logger.Error("Csc Global Synchronize Task failed!");
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

        private void SendEnrollmentEmail(string poNumber, List<DcvDetail> dvcDetails)
        {
            var bodyLines = new List<string>
            {
                $"PO Number: {poNumber}"
            };

            bodyLines.Add($"Domain Name: {String.Join(",", dvcDetails.Select(x => x.DomainName))}");
            bodyLines.Add($"CName Name: {String.Join(",", dvcDetails.Select(x => x.CName.Name))}");
            bodyLines.Add($"CName Value: {String.Join(",", dvcDetails.Select(x => x.CName.Value))}");

            var enrollSubject = $"Keyfactor PO {poNumber} Certificate is Ready for CNAME Processing";
            _requestManager.SendEmail(SmtpEmailHost, FromEmailAddress,
                CscGlobalEmail, enrollSubject, EmailUserId, EmailPassword, EmailPort, bodyLines);
        }

        public override EnrollmentResult Enroll(ICertificateDataReader certificateDataReader, string csr,
            string subject, Dictionary<string, string[]> san, EnrollmentProductInfo productInfo,
            PKIConstants.X509.RequestFormat requestFormat, RequestUtilities.EnrollmentType enrollmentType)
        {

            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);

            RegistrationRequest enrollmentRequest;
            CAConnectorCertificate priorCert;
            ReissueRequest reissueRequest;
            RenewalRequest renewRequest;

            string uUId;
            switch (enrollmentType)
            {
                case RequestUtilities.EnrollmentType.New:
                    Logger.Trace("Entering New Enrollment");
                    //If they renewed an expired cert it gets here and this will not be supported
                    IRegistrationResponse enrollmentResponse;
                    if (!productInfo.ProductParameters.ContainsKey("PriorCertSN"))
                    {
                        enrollmentRequest = _requestManager.GetRegistrationRequest(productInfo, csr, san);
                        Logger.Trace($"Enrollment Request JSON: {JsonConvert.SerializeObject(enrollmentRequest)}");
                        enrollmentResponse =
                            Task.Run(async () => await CscGlobalClient.SubmitRegistrationAsync(enrollmentRequest))
                                .Result;

                        if (enrollmentResponse?.Result != null)
                        {
                            var poNumber = enrollmentRequest.CustomFields
                                .FirstOrDefault(s => s.Name == "Purchase Order Number")?.Value;
                            var dvcDetails = enrollmentResponse.Result.DcvDetails;
                            if (productInfo.ProductParameters["Domain Control Validation Method"] == "CNAME")
                                SendEnrollmentEmail(poNumber, dvcDetails);
                        }
                        else
                        {
                            return new EnrollmentResult
                            {
                                Status = 30, //failure
                                StatusMessage = $"Enrollment Failed {enrollmentResponse?.RegistrationError.Description}"
                            };
                        }

                        Logger.Trace($"Enrollment Response JSON: {JsonConvert.SerializeObject(enrollmentResponse)}");

                    }
                    else
                    {
                        return new EnrollmentResult
                        {
                            Status = 30, //failure
                            StatusMessage = "You cannot renew and expired cert please perform an new enrollment."
                        };
                    }

                    Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                    return _requestManager.GetEnrollmentResult(enrollmentResponse);
                case RequestUtilities.EnrollmentType.Renew:
                    Logger.Trace("Entering Renew Enrollment");
                    //One click won't work for this implementation b/c we are missing enrollment params
                    if (productInfo.ProductParameters.ContainsKey("Applicant Last Name"))
                    {
                        priorCert = certificateDataReader.GetCertificateRecord(
                            DataConversion.HexToBytes(productInfo.ProductParameters["PriorCertSN"]));
                        uUId = priorCert.CARequestID.Substring(0, 36); //uUId is a GUID
                        Logger.Trace($"Renew uUId: {uUId}");
                        renewRequest = _requestManager.GetRenewalRequest(productInfo, uUId, csr, san);
                        Logger.Trace($"Renewal Request JSON: {JsonConvert.SerializeObject(renewRequest)}");
                        var renewResponse = Task.Run(async () => await CscGlobalClient.SubmitRenewalAsync(renewRequest))
                            .Result;
                        if (renewResponse?.Result != null)
                        {
                            var poNumber = renewRequest.CustomFields
                                .FirstOrDefault(s => s.Name == "Purchase Order Number")?.Value;
                            var dvcDetails = renewResponse.Result.DcvDetails;
                            if (productInfo.ProductParameters["Domain Control Validation Method"] == "CNAME")
                                SendEnrollmentEmail(poNumber, dvcDetails);

                            Logger.Trace($"Renewal Response JSON: {JsonConvert.SerializeObject(renewResponse)}");
                        }
                        else
                        {
                            return new EnrollmentResult
                            {
                                Status = 30, //failure
                                StatusMessage = $"Enrollment Failed {renewResponse?.RegistrationError.Description}"
                            };
                        }

                        Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                        return _requestManager.GetRenewResponse(renewResponse);
                    }
                    else
                    {
                        return new EnrollmentResult
                        {
                            Status = 30, //failure
                            StatusMessage =
                                "One click Renew Is Not Available for this Certificate Type.  Use the configure button instead."
                        };
                    }


                case RequestUtilities.EnrollmentType.Reissue:
                    Logger.Trace("Entering Reissue Enrollment");
                    //One click won't work for this implementation b/c we are missing enrollment params
                    if (productInfo.ProductParameters.ContainsKey("Applicant Last Name"))
                    {
                        priorCert = certificateDataReader.GetCertificateRecord(
                            DataConversion.HexToBytes(productInfo.ProductParameters["PriorCertSN"]));
                        uUId = priorCert.CARequestID.Substring(0, 36); //uUId is a GUID
                        Logger.Trace($"Reissue uUId: {uUId}");
                        reissueRequest = _requestManager.GetReissueRequest(productInfo, uUId, csr, san);
                        Logger.Trace($"Reissue JSON: {JsonConvert.SerializeObject(reissueRequest)}");
                        var reissueResponse = Task
                            .Run(async () => await CscGlobalClient.SubmitReissueAsync(reissueRequest))
                            .Result;
                        if (reissueResponse?.Result != null)
                        {
                            var poNumber = reissueRequest.CustomFields
                                .FirstOrDefault(s => s.Name == "Purchase Order Number")?.Value;
                            var dvcDetails = reissueResponse.Result.DcvDetails;
                            if (productInfo.ProductParameters["Domain Control Validation Method"] == "CNAME")
                                SendEnrollmentEmail(poNumber, dvcDetails);

                            Logger.Trace($"Reissue Response JSON: {JsonConvert.SerializeObject(reissueResponse)}");
                        }
                        else
                        {
                            return new EnrollmentResult
                            {
                                Status = 30, //failure
                                StatusMessage = $"Enrollment Failed {reissueResponse?.RegistrationError.Description}"
                            };
                        }

                        Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                        return _requestManager.GetReIssueResult(reissueResponse);
                    }
                    else
                    {
                        return new EnrollmentResult
                        {
                            Status = 30, //failure
                            StatusMessage =
                                "One click Renew Is Not Available for this Certificate Type.  Use the configure button instead."
                        };
                    }
            }

            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
            return null;
        }


        public override CAConnectorCertificate GetSingleRecord(string caRequestId)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            var keyfactorCaId = caRequestId.Substring(0, 36); //todo fix to use pipe delimiter
            Logger.Trace($"Keyfactor Ca Id: {keyfactorCaId}");
            var certificateResponse =
                Task.Run(async () => await CscGlobalClient.SubmitGetCertificateAsync(keyfactorCaId))
                    .Result;

            Logger.Trace($"Single Cert JSON: {JsonConvert.SerializeObject(certificateResponse)}");
            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
            return new CAConnectorCertificate
            {
                CARequestID = keyfactorCaId,
                Certificate = certificateResponse.Certificate,
                Status = _requestManager.MapReturnStatus(certificateResponse.Status),
                SubmissionDate = Convert.ToDateTime(certificateResponse.OrderDate)
            };
        }

        public override void Initialize(ICAConnectorConfigProvider configProvider)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            CscGlobalClient = new CscGlobalClient(configProvider);
            KeyfactorClient = new KeyfactorClient(configProvider);
            CscGlobalEmail = configProvider.CAConnectionData[Constants.CscGlobalEmailString].ToString();
            SmtpEmailHost = configProvider.CAConnectionData[Constants.SmtpEmailHost].ToString();
            FromEmailAddress = configProvider.CAConnectionData[Constants.FromEmailAddress].ToString();
            EmailUserId = configProvider.CAConnectionData[Constants.EmailUserId].ToString();
            EmailPassword = configProvider.CAConnectionData[Constants.EmailPassword].ToString();
            EmailPort = Convert.ToInt16(configProvider.CAConnectionData[Constants.EmailPort].ToString());
            var templateSync = configProvider.CAConnectionData["TemplateSync"].ToString();
            if (templateSync.ToUpper() == "ON") EnableTemplateSync = true;
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