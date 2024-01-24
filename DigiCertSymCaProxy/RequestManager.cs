// Copyright 2023 Keyfactor                                                   
// Licensed under the Apache License, Version 2.0 (the "License"); you may    
// not use this file except in compliance with the License.  You may obtain a 
// copy of the License at http://www.apache.org/licenses/LICENSE-2.0.  Unless 
// required by applicable law or agreed to in writing, software distributed   
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES   
// OR CONDITIONS OF ANY KIND, either express or implied. See the License for  
// thespecific language governing permissions and limitations under the       
// License. 
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using CAProxy.AnyGateway.Models;
using CSS.Common.Logging;
using CSS.PKI;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.DigicertMPKISOAP;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using System.Linq;

namespace Keyfactor.AnyGateway.DigiCertSym
{
    public class RequestManager : LoggingClientBase
    {

        public enum KeyfactorRevokeReasons : uint
        {
            KeyCompromised = 1,
            AffiliationChanged = 3,
            Superseded = 4,
            CessationOfOperation = 5
        }

        public string DnsConstantName { get; set; }
        public string UpnConstantName { get; set; }
        public string IpConstantName { get; set; }
        public string EmailConstantName { get; set; }
        public int OuStartPoint { get; set; }


        public static Func<string, string> Pemify = ss =>
            ss.Length <= 64 ? ss : ss.Substring(0, 64) + "\n" + Pemify(ss.Substring(64));

        public int MapReturnStatus(string digiCertStatus)
        {
            try
            {
                Logger.Debug("Entering MapReturnStatus(string digiCertStatus) Method...");
                Logger.Trace($"digiCertStatus is {digiCertStatus}");
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
                Logger.Trace($"returnStatus is {returnStatus}");
                Logger.Debug("Exiting MapReturnStatus(string digiCertStatus) Method...");
                return Convert.ToInt32(returnStatus);
            }
            catch (Exception e)
            {
                Logger.Error($"Exception Occurred in MapReturnStatus(string digiCertStatus): {e.Message}");
                throw;
            }
        }

        public int MapReturnStatus(CertificateStatusEnum digiCertStatus)
        {
            try
            {
                Logger.Debug("Entering MapReturnStatus(string digiCertStatus) Method...");
                Logger.Trace($"digiCertStatus is {digiCertStatus}");
                PKIConstants.Microsoft.RequestDisposition returnStatus;

                switch (digiCertStatus)
                {
                    case CertificateStatusEnum.VALID:
                        returnStatus = PKIConstants.Microsoft.RequestDisposition.ISSUED;
                        break;
                    case CertificateStatusEnum.EXPIRED:
                        returnStatus = PKIConstants.Microsoft.RequestDisposition.ISSUED;
                        break;
                    case CertificateStatusEnum.SUSPENDED:
                        returnStatus = PKIConstants.Microsoft.RequestDisposition.REVOKED;
                        break;
                    case CertificateStatusEnum.REVOKED:
                        returnStatus = PKIConstants.Microsoft.RequestDisposition.REVOKED;
                        break;
                    default:
                        returnStatus = PKIConstants.Microsoft.RequestDisposition.UNKNOWN;
                        break;
                }
                Logger.Trace($"returnStatus is {returnStatus}");
                Logger.Debug("Exiting MapReturnStatus(string digiCertStatus) Method...");
                return Convert.ToInt32(returnStatus);
            }
            catch (Exception e)
            {
                Logger.Error($"Exception Occurred in MapReturnStatus(string digiCertStatus): {e.Message}");
                throw;
            }
        }

        public RevokeRequest GetRevokeRequest(uint kfRevokeReason)
        {
            try
            {
                Logger.Debug("Entering GetRevokeRequest(uint kfRevokeReason) Method...");
                Logger.Trace($"kfRevokeReason is {kfRevokeReason}");
                var req = new RevokeRequest { RevocationReason = MapRevokeReason(kfRevokeReason) };
                Logger.Trace($"Revoke Request JSON {JsonConvert.SerializeObject(req)}");
                Logger.Debug("Exiting GetRevokeRequest(uint kfRevokeReason) Method...");
                return req;
            }
            catch (Exception e)
            {
                Logger.Error($"Exception Occurred in GetRevokeRequest(uint kfRevokeReason): {e.Message}");
                throw;
            }
        }


        public string MapRevokeReason(uint kfRevokeReason)
        {
            try
            {
                Logger.Debug("Entering MapRevokeReason(uint kfRevokeReason) Method...");
                Logger.Trace($"kfRevokeReason is {kfRevokeReason}");
                Logger.Debug("Exiting MapRevokeReason(uint kfRevokeReason) Method...");
                switch (kfRevokeReason)
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
            catch (Exception e)
            {
                Logger.Error($"Exception Occurred in MapRevokeReason(uint kfRevokeReason): {e.Message}");
                throw;
            }
        }

        internal int MapSoapRevokeReason(RevokeReasonCodeEnum revokeReason)
        {
            try
            {
                Logger.Debug("Entering MapRevokeReason(uint kfRevokeReason) Method...");
                Logger.Trace($"dcRevokeReason is {revokeReason}");
                Logger.Debug("Exiting MapRevokeReason(uint kfRevokeReason) Method...");
                switch (revokeReason)
                {
                    case RevokeReasonCodeEnum.KeyCompromise:
                        return (int)KeyfactorRevokeReasons.KeyCompromised;
                    case RevokeReasonCodeEnum.CessationOfOperation:
                        return (int)KeyfactorRevokeReasons.CessationOfOperation;
                    case RevokeReasonCodeEnum.AffiliationChanged:
                        return (int)KeyfactorRevokeReasons.AffiliationChanged;
                    case RevokeReasonCodeEnum.Superseded:
                        return (int)KeyfactorRevokeReasons.Superseded;
                }
                return 0;
            }
            catch (Exception e)
            {
                Logger.Error($"Exception Occurred in MapRevokeReason(uint kfRevokeReason): {e.Message}");
                throw;
            }
        }

        public int GetRevokeResult(IRevokeResponse revokeResponse)
        {
            try
            {
                Logger.Debug("Entering GetRevokeResult(IRevokeResponse revokeResponse) Method...");
                if (revokeResponse.RegistrationError != null)
                    return Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.FAILED);
                Logger.Debug("Exiting GetRevokeResult(IRevokeResponse revokeResponse) Method...");
                return Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.REVOKED);
            }
            catch (Exception e)
            {
                Logger.Error($"Exception Occurred in GetRevokeResult(IRevokeResponse revokeResponse): {e.Message}");
                throw;
            }
        }

        public SearchCertificateRequestType GetSearchCertificatesRequest(int pageCounter, string templateId)
        {
            try
            {
                Logger.Debug("Entering GetSearchCertificatesRequest(int pageCounter, string templateId) Method...");
                Logger.Debug("Exiting GetSearchCertificatesRequest(int pageCounter, string templateId) Method...");
                Logger.Trace($"pageCounter: {pageCounter} TemplateId: {templateId}");
                return new SearchCertificateRequestType
                {
                    profileOID = templateId,
                    startIndex = pageCounter,
                    startIndexSpecified = true,
                    version = "1.0"
                };
            }
            catch (Exception e)
            {
                Logger.Error($"Exception Occurred in GetSearchCertificatesRequest(int pageCounter, string seatId): {e.Message}");
                throw;
            }
        }

        private (Dictionary<string, string> DNSOut, Dictionary<string, string> MultiOut) ProcessSansArray(
             string[] sanArray, string commonName)
        {
            Dictionary<string, string> dnsOut = new Dictionary<string, string>();
            Dictionary<string, string> multiOut = new Dictionary<string, string>();

            if (sanArray.Length == 1)
            {
                var singleItem = sanArray.First();
                if (singleItem == commonName || string.IsNullOrWhiteSpace(commonName))
                {
                    dnsOut.Add(singleItem, singleItem);
                }
                else
                {
                    throw new InvalidOperationException("Error: Single item does not match CommonName.");
                }
            }
            else if (sanArray.Length > 1)
            {
                if (!string.IsNullOrWhiteSpace(commonName))
                {
                    if (!sanArray.Contains(commonName))
                    {
                        throw new InvalidOperationException("Error: Multiple items, none of them match CommonName.");
                    }
                    else
                    {
                        dnsOut.Add(commonName, commonName);
                        multiOut = sanArray.Where(item => item != commonName)
                            .ToDictionary(item => item, item => item);
                    }
                }
                else
                {
                    dnsOut.Add(sanArray.First(), sanArray.First());
                    multiOut = sanArray.Skip(1).ToDictionary(item => item, item => item);
                }
            }

            return (dnsOut, multiOut);
        }

        public EnrollmentRequest GetEnrollmentRequest(EnrollmentProductInfo productInfo, string csr,
            Dictionary<string, string[]> san)
        {
            try
            {
                Logger.Debug("Entering GetEnrollmentRequest(EnrollmentProductInfo productInfo, string csr,Dictionary<string, string[]> san) Method...");
                Logger.Trace($"csr: {csr}");
                var pemCert = Pemify(csr);
                Logger.Trace($"pemCert Intermediate: {pemCert}");
                pemCert = "-----BEGIN CERTIFICATE REQUEST-----\n" + pemCert;
                pemCert += "\n-----END CERTIFICATE REQUEST-----";
                Logger.Trace($"pemCertFinal: {pemCert}");

                var sn = new San();
                CertificationRequestInfo csrParsed;

                using (TextReader sr = new StringReader(pemCert))
                {
                    var reader = new PemReader(sr);
                    var cReq = reader.ReadObject() as Pkcs10CertificationRequest;
                    csrParsed = cReq?.GetCertificationRequestInfo();
                }
                Logger.Trace($"Parsed CSR Subject Value Is: {csrParsed?.Subject.ToString().Split(',')}");

                Logger.Trace("Getting File Execution Location to retrieve path");
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                path = Path.GetDirectoryName(path) + "\\";
                Logger.Trace($"Executing path for the file is: {path}");

                Logger.Trace($"Reading in JSON template to parse file {productInfo.ProductParameters["EnrollmentTemplate"]}");
                JObject jsonTemplate = JObject.Parse(File.ReadAllText(path + productInfo.ProductParameters["EnrollmentTemplate"]));
                var jsonResult = jsonTemplate.ToString();
                Logger.Trace($"Read in JSON, resulting template: {jsonResult}");

                //1. Loop through list of Product Parameters and replace in JSON
                foreach (var productParam in productInfo.ProductParameters)
                {
                    jsonResult = ReplaceProductParam(productParam, jsonResult);
                }
                //Clean up the Numeric values remove double quotes
                jsonResult = jsonResult.Replace("\"Numeric|", "");
                jsonResult = jsonResult.Replace("|Numeric\"", "");
                Logger.Trace($"Replaced product params, result: {jsonResult}");

                //2. Loop though list of Parsed CSR Elements and replace in JSON
                var csrValues = csrParsed?.Subject.ToString().Split(',');

                bool getCommonNameFromSubject = csrValues != null && csrValues[0].Length > 0;

                //certBot workflow, common name always comes only through SAN and is not in common name
                if (csrValues != null && getCommonNameFromSubject)
                    foreach (var csrValue in csrValues)
                    {
                        var nmValPair = csrValue.Split('=');
                        jsonResult = ReplaceCsrEntry(nmValPair, jsonResult);
                    }

                Logger.Trace($"Replaced CSR elements, result: {jsonResult}");

                //3. Replace the RAW CSR content
                jsonResult = jsonResult.Replace("CSR|RAW", csr);

                Logger.Trace($"Replaced RAW CSR String, result: {jsonResult}");

                //4. Deserialize Back to EnrollmentRequest
                var enrollmentRequest = JsonConvert.DeserializeObject<EnrollmentRequest>(jsonResult);

                Logger.Trace($"Enrollment Serialized JSON before DNS and OU, result: {JsonConvert.SerializeObject(enrollmentRequest)}");

                Dictionary<string, string> MultiOut=null;

                List<DnsName> dnsList = new List<DnsName>();

                //5. If it contains the dns and it is not multi domain get the DNS
                if (san.ContainsKey("dns"))
                {
                    var dnsKp = san["dns"];
                    Logger.Trace($"dnsKP: {dnsKp}");

                    (Dictionary<string, string> DNSOut, Dictionary<string, string> MultiOut) result;

                    if (!getCommonNameFromSubject)
                    {
                        //Cert Bot flow, Cert Bot has no common name and the dns comes from the SAN blank for common name returns first DNS
                        result = ProcessSansArray(dnsKp, "");
                    }
                    else
                    {
                        result = ProcessSansArray(dnsKp, enrollmentRequest?.Attributes?.CommonName);
                    }

                    DnsName up = new DnsName { Id = DnsConstantName, Value = result.DNSOut.FirstOrDefault().Value };

                    MultiOut = result.MultiOut;
                    var jsonResultDns = JsonConvert.SerializeObject(enrollmentRequest);

                    if (!getCommonNameFromSubject)
                        jsonResultDns = ReplaceCsrEntry(new[] { "CN", result.DNSOut.FirstOrDefault().Value }, jsonResult);

                    enrollmentRequest = JsonConvert.DeserializeObject<EnrollmentRequest>(jsonResultDns);
                    dnsList.Add(up);

                    //5. Handle the multiple domain scenario domains go in a different attribute
                    if (MultiOut?.Count > 0)
                    {
                        DnsName mdns = new DnsName { Id = "custom_encode_dnsName_multi", Value = string.Join(",", MultiOut.Values) };
                        dnsList.Add(mdns);
                    }

                    sn.DnsName = dnsList;
                }

                //6. Loop through User Principal Entries
                if (san.ContainsKey("upn"))
                {
                    var upList = new List<UserPrincipalName>();
                    var upKp = san["upn"];

                    Logger.Trace($"upn: {upKp}");

                    //Multiple UPNs not supported by Digicert so take the first one in the list
                    UserPrincipalName up = new UserPrincipalName { Id = UpnConstantName, Value = upKp.FirstOrDefault() };
                    upList.Add(up);
                    sn.UserPrincipalName = upList;
                }

                //7. Loop through IP Entries
                if (san.ContainsKey("ipaddress"))
                {
                    var ipList = new List<IpAddress>();

                    var ipKp = san["ipaddress"];
                    Logger.Trace($"ip: {ipKp}");

                    //Multiple IP Addresses not supported by Digicert so take the first one in the list
                    IpAddress ip = new IpAddress { Id = IpConstantName, Value = ipKp.FirstOrDefault() };
                    ipList.Add(ip);
                    sn.IpAddress = ipList;
                }

                //8. Loop through mail Entries
                if (san.ContainsKey("mail"))
                {
                    var mailList = new List<Rfc822Name>();
                    var mailKp = san["mail"];

                    Logger.Trace($"mail: {mailKp}");

                    //Multiple IP Addresses not supported by Digicert so take the first one in the list
                    Rfc822Name mail = new Rfc822Name { Id = EmailConstantName, Value = mailKp.FirstOrDefault() };
                    mailList.Add(mail);
                    sn.Rfc822Name = mailList;
                }

                //9. Loop through OUs and replace in Object
                var organizationUnitsRaw = GetValueFromCsr("OU", csrParsed);
                Logger.Trace($"Raw Organizational Units: {organizationUnitsRaw}");
                var organizationalUnits = organizationUnitsRaw.Split('/');
                var orgUnits = new List<OrganizationUnit>();
                Logger.Trace($"OuStartPoint is {OuStartPoint}");
                var i = OuStartPoint;
                foreach (var ou in organizationalUnits)
                {
                    var organizationUnit = new OrganizationUnit { Id = OuStartPoint == 0 ? "cert_org_unit" : "cert_org_unit" + i, Value = ou };
                    orgUnits.Add(organizationUnit);
                    i++;
                }

                var attributes = enrollmentRequest.Attributes;
                attributes.OrganizationUnit = orgUnits;
                attributes.San = sn;
                enrollmentRequest.Attributes = attributes;
                Logger.Trace($"Final enrollmentRequest: {JsonConvert.SerializeObject(enrollmentRequest)}");
                Logger.Debug("Exiting GetEnrollmentRequest(EnrollmentProductInfo productInfo, string csr,Dictionary<string, string[]> san) Method...");
                return enrollmentRequest;
            }
            catch (Exception e)
            {
                Logger.Error($"Error In GetEnrollmentRequest(EnrollmentProductInfo productInfo, string csr,Dictionary<string, string[]> san) : {e.Message}");
                throw;
            }

        }

        public EnrollmentResult
            GetEnrollmentResult(
                IEnrollmentResponse enrollmentResponse, CAConnectorCertificate cert)
        {
            try
            {
                Logger.Debug("Entering/Exiting GetEnrollmentResult(IEnrollmentResponse enrollmentResponse) Method...");
                if (enrollmentResponse.RegistrationError != null)
                    return new EnrollmentResult
                    {
                        Status = (int)PKIConstants.Microsoft.RequestDisposition.FAILED, //failure
                        StatusMessage = "Error occurred when enrolling"
                    };

                return new EnrollmentResult
                {
                    Status = (int)PKIConstants.Microsoft.RequestDisposition.ISSUED, //success
                    CARequestID = enrollmentResponse.Result.SerialNumber,
                    Certificate = cert.Certificate,
                    StatusMessage =
                        $"Order Successfully Created With Order Number {enrollmentResponse.Result.SerialNumber}"
                };
            }
            catch (Exception e)
            {
                Logger.Error($"Error in GetEnrollmentResult(IEnrollmentResponse enrollmentResponse) Method: {e.Message}");
                throw;
            }
        }

        internal string FlattenErrors(List<ErrorResponse> errors)
        {
            try
            {
                Logger.Debug("Entering in FlattenErrors(List<ErrorResponse> errors) Method...");
                var errorMessage = string.Empty;
                foreach (var error in errors) errorMessage += "Code: " + error.Code + " Message: " + error.Message + "Field Name: " + error.Field + "\n";
                Logger.Debug("Exiting in FlattenErrors(List<ErrorResponse> errors) Method...");
                return errorMessage;
            }
            catch (Exception e)
            {
                Logger.Error($"Error in FlattenErrors(List<ErrorResponse> errors) Method: {e.Message}");
                throw;
            }
        }

        internal EnrollmentResult GetRenewResponse(EnrollmentResponse renewResponse, CAConnectorCertificate cert)
        {
            try
            {
                Logger.Debug("Entering/Exiting in GetRenewResponse(EnrollmentResponse renewResponse) Method...");
                if (renewResponse.RegistrationError != null)
                    return new EnrollmentResult
                    {
                        Status = (int)PKIConstants.Microsoft.RequestDisposition.FAILED, //failure
                        StatusMessage = "Error occurred when enrolling"
                    };

                return new EnrollmentResult
                {
                    Status = (int)PKIConstants.Microsoft.RequestDisposition.ISSUED, //success
                    CARequestID = renewResponse.Result.SerialNumber,
                    Certificate = cert.Certificate,
                    StatusMessage =
                        $"Order Successfully Created With Order Number {renewResponse.Result.SerialNumber}"
                };
            }
            catch (Exception e)
            {
                Logger.Error($"Error in GetRenewResponse(EnrollmentResponse renewResponse) Method: {e.Message}");
                throw;
            }
        }

        public string GetValueFromCsr(string subjectItem, CertificationRequestInfo csr)
        {
            try
            {
                Logger.Debug("Entering in GetValueFromCsr(string subjectItem, CertificationRequestInfo csr) Method...");
                var csrValues = csr.Subject.ToString().Split(',');
                foreach (var val in csrValues)
                {
                    var nmValPair = val.Split('=');
                    Logger.Trace($"nmValPair {nmValPair}");
                    if (subjectItem == nmValPair[0]) return nmValPair[1];
                }
                Logger.Debug("Exiting in GetValueFromCsr(string subjectItem, CertificationRequestInfo csr) Method...");
                return "";
            }
            catch (Exception e)
            {
                Logger.Error($"Error in GetValueFromCsr(string subjectItem, CertificationRequestInfo csr) Method: {e.Message}");
                throw;
            }
        }

        private string ReplaceProductParam(KeyValuePair<string, string> productParam, string jsonResult)
        {
            try
            {
                Logger.Debug("Entering ReplaceProductParam(KeyValuePair<string, string> productParam, string jsonResult) Method...");
                return jsonResult.Replace("EnrollmentParam|" + productParam.Key, productParam.Value);
            }
            catch (Exception e)
            {
                Logger.Error($"Error in ReplaceProductParam(KeyValuePair<string, string> productParam, string jsonResult) Method: {e.Message}");
                throw;
            }
        }

        private string ReplaceCsrEntry(string[] nameValuePair, string jsonResult)
        {
            try
            {
                Logger.Debug("Entering in ReplaceCsrEntry(string[] nameValuePair, string jsonResult) Method...");
                string pattern = @"\b" + "CSR\\|" + nameValuePair[0] + @"\b";
                string replace = nameValuePair[1];
                Logger.Trace($"replace {replace}");
                Logger.Debug("Exiting in ReplaceCsrEntry(string[] nameValuePair, string jsonResult) Method...");
                return Regex.Replace(jsonResult, pattern, replace);
            }
            catch (Exception e)
            {
                Logger.Error($"Error in ReplaceCsrEntry(string[] nameValuePair, string jsonResult) Method: {e.Message}");
                throw;
            }
        }
    }
}