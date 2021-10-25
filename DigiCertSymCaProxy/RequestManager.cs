using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CAProxy.AnyGateway.Models;
using CSS.PKI;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

namespace Keyfactor.AnyGateway.DigiCertSym
{
    public class RequestManager 
    {

        public enum KeyfactorRevokeReasons : uint
        {
            KeyCompromised = 1,
            AffiliationChanged = 3,
            Superseded = 4,
            CessationOfOperation = 5
        }

        public static Func<string, string> Pemify = ss =>
            ss.Length <= 64 ? ss : ss.Substring(0, 64) + "\n" + Pemify(ss.Substring(64));

        public int MapReturnStatus(string digiCertStatus)
        {
            PKIConstants.Microsoft.RequestDisposition returnStatus;

            switch (digiCertStatus)
            {
                case "VALID":
                    returnStatus = PKIConstants.Microsoft.RequestDisposition.ISSUED;
                    break;
                case "Initial":
                case "PENDING":
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
            var req = new RevokeRequest {RevocationReason = MapRevokeReason(kfRevokeReason)};
            return req;
        }


        private string MapRevokeReason(uint kfRevokeReason)
        {
            switch (kfRevokeReason)
            {
                case (uint) KeyfactorRevokeReasons.KeyCompromised:
                    return DigiCertRevokeReasons.KeyCompromise;
                case (uint) KeyfactorRevokeReasons.CessationOfOperation:
                    return DigiCertRevokeReasons.CessationOfOperation;
                case (uint) KeyfactorRevokeReasons.AffiliationChanged:
                    return DigiCertRevokeReasons.AffiliationChanged;
                case (uint) KeyfactorRevokeReasons.Superseded:
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

        public CertificateSearchRequest GetSearchCertificatesRequest(int pageCounter, string seatId)
        {
            return new CertificateSearchRequest
            {
                SeatId = seatId,
                StartIndex = pageCounter
            };
        }

        public EnrollmentRequest GetEnrollmentRequest(EnrollmentProductInfo productInfo, string csr,
            Dictionary<string, string[]> san) //todo Make this more flexible to support all the template configuration combinations
        {
            var pemCert = Pemify(csr);
            pemCert = "-----BEGIN CERTIFICATE REQUEST-----\n" + pemCert;
            pemCert += "\n-----END CERTIFICATE REQUEST-----";

            //var req = new EnrollmentRequest();
            var sn = new San();
            CertificationRequestInfo csrParsed;

            using (TextReader sr = new StringReader(pemCert))
            {
                var reader = new PemReader(sr);
                var cReq = reader.ReadObject() as Pkcs10CertificationRequest;
                csrParsed = cReq?.GetCertificationRequestInfo();
            }
            
            string path = Directory.GetCurrentDirectory();
            JObject jsonTemplate = JObject.Parse(File.ReadAllText(path + productInfo.ProductParameters["EnrollmentTemplate"]));
            var jsonResult = jsonTemplate.ToString();

            //1. Loop through list of Product Parameters and replace in JSON
            foreach (var productParam in productInfo.ProductParameters)
            {
                jsonResult = ReplaceProductParam(productParam, jsonResult);
            }
            //Clean up the Numeric values remove double quotes
            jsonResult = jsonResult.Replace("\"Numeric|", "");
            jsonResult = jsonResult.Replace("|Numeric\"", "");

            //2. Loop though list of Parsed CSR Elements and replace in JSON
            var csrValues = csrParsed?.Subject.ToString().Split(',');
            if (csrValues != null)
                foreach (var csrValue in csrValues)
                {
                    var nmValPair = csrValue.Split('=');
                    jsonResult = ReplaceCsrEntry(nmValPair, jsonResult);
                }

            //3. Replace the RAW CSR content
            jsonResult = jsonResult.Replace("CSR|RAW", csr);

            //4. Deserialize Back to EnrollmentRequest
            var enrollmentRequest = JsonConvert.DeserializeObject<EnrollmentRequest>(jsonResult);

            //5. Loop through SANS and replace in Object
            var dnsList = new List<DnsName>();
            var dnsKp = san["DNS Name"];
            foreach (var item in dnsKp)
            {
                DnsName dns = new DnsName { Value = item };
                dnsList.Add(dns);
            }

            //6. Loop through OUs and replace in Object
            var organizationalUnits = GetValueFromCsr("OU", csrParsed).Split('/');

            var orgUnits = new List<OrganizationUnit>();
            var i = 1;
            foreach (var ou in organizationalUnits)
            {
                var organizationUnit = new OrganizationUnit { Id = "cert_org_unit" + i, Value = ou };
                orgUnits.Add(organizationUnit);
                i++;
            }


            var attributes = enrollmentRequest.Attributes;
            attributes.OrganizationUnit = orgUnits;
            sn.DnsName = dnsList;
            attributes.San = sn;
            enrollmentRequest.Attributes = attributes;

            return enrollmentRequest;
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
            var errorMessage = string.Empty;
            foreach (var error in errors) errorMessage += error.Message + "\n";

            return errorMessage;
        }

        internal EnrollmentResult GetRenewResponse(EnrollmentResponse renewResponse)
        {
            if (renewResponse.RegistrationError != null)
                return new EnrollmentResult
                {
                    Status = 30, //failure
                    StatusMessage = "Error occurred when enrolling"
                };

            return new EnrollmentResult
            {
                Status = 13, //success
                CARequestID = renewResponse.Result.SerialNumber,
                StatusMessage =
                    $"Order Successfully Created With Order Number {renewResponse.Result.SerialNumber}"
            };
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

        private static string ReplaceProductParam(KeyValuePair<string, string> productParam, string jsonResult)
        {
            return jsonResult.Replace("EnrollmentParam|" + productParam.Key, productParam.Value);
        }

        private static string ReplaceCsrEntry(string[] nameValuePair, string jsonResult)
        {
            string pattern = @"\b" + "CSR\\|" + nameValuePair[0] + @"\b";
            string replace = nameValuePair[1];
            return Regex.Replace(jsonResult, pattern, replace);
        }
    }
}