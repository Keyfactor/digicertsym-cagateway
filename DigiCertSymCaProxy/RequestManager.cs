using System;
using System.Collections.Generic;
using CAProxy.AnyGateway.Models;
using CSS.PKI;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;

namespace Keyfactor.AnyGateway.DigiCertSym
{
    internal class RequestManager
    {
        public int MapReturnStatus(string digicertStatus)
        {
            PKIConstants.Microsoft.RequestDisposition returnStatus;

            switch (digicertStatus)
            {
                case "ACTIVE":
                    returnStatus = PKIConstants.Microsoft.RequestDisposition.ISSUED;
                    break;
                case "Initial":
                case "Pending":
                    returnStatus = PKIConstants.Microsoft.RequestDisposition.PENDING;
                    break;
                case "REVOKED":
                    returnStatus = PKIConstants.Microsoft.RequestDisposition.REVOKED;
                    break;
                default:
                    returnStatus = PKIConstants.Microsoft.RequestDisposition.UNKNOWN;
                    break;
            }

            return Convert.ToInt32(returnStatus);
        }


        public int GetRevokeResult(IRevokeResponse revokeResponse)
        {
            if (revokeResponse.RegistrationError != null)
                return Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.FAILED);

            return Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.REVOKED);
        }

        public EnrollmentRequest GetEnrollmentRequest(EnrollmentProductInfo productInfo, string csr, Dictionary<string, string[]> san)
        {
            throw new NotImplementedException();
        }

        public EnrollmentResult
            GetEnrollmentResult(
                IEnrollmentResponse enrollmentResponse)
        {
            if (enrollmentResponse.RegistrationError != null)
                return new EnrollmentResult
                {
                    Status = 30, //failure
                    StatusMessage = "Error occurred when enrolling"
                };

            return new EnrollmentResult
            {
                Status = 13, //success
                CARequestID = enrollmentResponse.Result.SerialNumber,
                StatusMessage =
                    $"Order Successfully Created With Order Number {enrollmentResponse.Result.SerialNumber}"
            };
        }

        public EnrollmentRequest GetRenewalRequest(EnrollmentProductInfo productInfo, string priorCertSn, string csr, Dictionary<string, string[]> san)
        {
            throw new NotImplementedException();
        }

        public EnrollmentResult GetRenewResponse(EnrollmentResponse renewResponse)
        {
            throw new NotImplementedException();
        }
    }


}