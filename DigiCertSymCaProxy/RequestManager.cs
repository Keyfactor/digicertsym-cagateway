using System;
using System.Collections.Generic;
using System.IO;
using CAProxy.AnyGateway.Models;
using CSS.PKI;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

namespace Keyfactor.AnyGateway.DigiCertSym
{
    internal class RequestManager
    {
        public static Func<string, string> Pemify = ss =>
            ss.Length <= 64 ? ss : ss.Substring(0, 64) + "\n" + Pemify(ss.Substring(64));

        public int MapReturnStatus(string digiCertStatus)
        {
            PKIConstants.Microsoft.RequestDisposition returnStatus;

            switch (digiCertStatus)
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

        public RevokeRequest GetRevokeRequest(uint kfRevokeReason)
        {
            var req = new RevokeRequest();
            req.RevocationReason = MapRevokeReason(kfRevokeReason);
            return req;
        }

        private string MapRevokeReason(uint kfRevokeReason)
        {
            switch(kfRevokeReason)
            {
                case (uint)KeyfactorRevokeReasons.KeyCompromised:
                    return DigiCertRevokeReasons.KeyCompromise;
                case (uint)KeyfactorRevokeReasons.CessationOfOperation:
                    return DigiCertRevokeReasons.CessationOfOperation;
                case (uint)KeyfactorRevokeReasons.AffiliationChanged:
                    return DigiCertRevokeReasons.AffiliationChanged;
                case (uint)KeyfactorRevokeReasons.Superseded:
                    return DigiCertRevokeReasons.Superseded;
            }

            return "";
        }

        public int GetRevokeResult(IRevokeResponse revokeResponse)
        {
            if (revokeResponse.RegistrationError != null)
                return Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.FAILED);

            return Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.REVOKED);
        }

        public EnrollmentRequest GetEnrollmentRequest(EnrollmentProductInfo productInfo,string csr,
            Dictionary<string, string[]> san)
        {
            var pemCert = Pemify(csr);
            pemCert = "-----BEGIN CERTIFICATE REQUEST-----\n" + pemCert;
            pemCert += "\n-----END CERTIFICATE REQUEST-----";

            var req = new EnrollmentRequest();
            var sn = new San();
            var attributes = new Attributes();

            using (TextReader sr = new StringReader(pemCert))
            {
                var reader = new PemReader(sr);
                var cReq = reader.ReadObject() as Pkcs10CertificationRequest;
                var csrParsed = cReq?.GetCertificationRequestInfo();

                var profile = new Profile {Id = productInfo.ProductID};
                req.Profile = profile;
                var seat = new Seat {SeatId = productInfo.ProductParameters["Seat"]};
                var validity = new Validity
                {
                    Unit = "years", Duration = Convert.ToInt32(productInfo.ProductParameters["Validity (Years)"])
                };
                attributes.CommonName = GetValueFromCsr("CN", csrParsed);
                attributes.Country = GetValueFromCsr("C", csrParsed);
                req.Validity = validity;
                req.Seat = seat;
                req.Csr = csr;
            }

            switch (productInfo.ProductID)
            {
                case "2.16.840.1.113733.1.16.1.2.3.6.1.1266772938":
                    var pn = new UserPrincipalName();
                    pn.Id = "otherNameUPN";
                    pn.Value = productInfo.ProductParameters["User Principal Name"];
                    var pnList = new List<UserPrincipalName>
                    {
                        pn
                    };
                    sn.UserPrincipalName = pnList;
                    attributes.San = sn;
                    break;
                case "2.16.840.1.113733.1.16.1.5.2.5.1.1266771486":
                    break;
            }

            req.Attributes = attributes;
            return req;
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

        internal string FlattenErrors(List<ErrorResponse> errors)
        {
            string errorMessage = String.Empty;
            foreach(var error in errors)
            {
                errorMessage += error.Message + "\n";
            }

            return errorMessage;
        }

        internal EnrollmentResult GetRenewResponse(EnrollmentResponse renewResponse)
        {
            throw new NotImplementedException();
        }

        public static string GetValueFromCsr(string subjectItem, CertificationRequestInfo csr)
        {
            var csrValues = csr.Subject.ToString().Split(',');
            foreach (var val in csrValues)
            {
                var nmValPair = val.Split('=');

                if (subjectItem == nmValPair[0]) return nmValPair[1];
            }

            return "";
        }


        public enum KeyfactorRevokeReasons:uint
        {
            ReasonUnspecified=0,
            KeyCompromised=1,
            CACompromised=2,
            AffiliationChanged=3,
            Superseded=4,
            CessationOfOperation=5,
            CertificateHold=6
        }

    }
}