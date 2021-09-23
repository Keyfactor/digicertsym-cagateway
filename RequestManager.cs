﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using CAProxy.AnyGateway.Models;
using CSS.PKI;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.Interfaces;

namespace Keyfactor.AnyGateway.DigiCertSym
{
    public class RequestManager
    {
        private List<CustomField> GetCustomFields(EnrollmentProductInfo productInfo)
        {
            var poNumber = new CustomField();
            poNumber.Name = "Purchase Order Number";
            poNumber.Value = productInfo.ProductParameters["Purchase Order Number"];
            var customFieldList = new List<CustomField>();
            customFieldList.Add(poNumber);

            return customFieldList;
        }

        public EnrollmentResult GetRenewResponse(RenewalResponse renewResponse)
        {
            if (renewResponse.RegistrationError != null)
                return new EnrollmentResult
                {
                    Status = 30, //failure
                    StatusMessage = renewResponse.RegistrationError.Description
                };

            return new EnrollmentResult
            {
                Status = 13, //success
                CARequestID = renewResponse.Result.Status.Uuid,
                StatusMessage = $"Renewal Successfully Completed For {renewResponse.Result.CommonName}"
            };
        }


        public EnrollmentResult
            GetEnrollmentResult(
                IRegistrationResponse registrationResponse)
        {
            if (registrationResponse.RegistrationError != null)
                return new EnrollmentResult
                {
                    Status = 30, //failure
                    StatusMessage = registrationResponse.RegistrationError.Description
                };

            return new EnrollmentResult
            {
                Status = 13, //success
                CARequestID = registrationResponse.Result.Status.Uuid,
                StatusMessage =
                    $"Order Successfully Created With Order Number {registrationResponse.Result.CommonName}"
            };
        }

        public int GetRevokeResult(IRevokeResponse revokeResponse)
        {
            if (revokeResponse.RegistrationError != null)
                return Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.FAILED);

            return Convert.ToInt32(PKIConstants.Microsoft.RequestDisposition.REVOKED);
        }

        public EnrollmentResult GetReIssueResult(IReissueResponse reissueResponse)
        {
            if (reissueResponse.RegistrationError != null)
                return new EnrollmentResult
                {
                    Status = 30, //failure
                    StatusMessage = reissueResponse.RegistrationError.Description
                };

            return new EnrollmentResult
            {
                Status = 13, //success
                CARequestID = reissueResponse.Result.Status.Uuid,
                StatusMessage = $"Reissue Successfully Completed For {reissueResponse.Result.CommonName}"
            };
        }

        public DomainControlValidation GetDomainControlValidation(string methodType, string[] emailAddress,
            string domainName)
        {
            foreach (var address in emailAddress)
            {
                var email = new MailAddress(address);
                if (domainName.Contains(email.Host.Split('.')[0]))
                    return new DomainControlValidation
                    {
                        MethodType = methodType,
                        EmailAddress = email.ToString()
                    };
            }

            return null;
        }

        public DomainControlValidation GetDomainControlValidation(string methodType, string emailAddress)
        {
            return new DomainControlValidation
            {
                MethodType = methodType,
                EmailAddress = emailAddress
            };
        }

        public RegistrationRequest GetRegistrationRequest(EnrollmentProductInfo productInfo, string csr,
            Dictionary<string, string[]> sans)
        {
            var bytes = Encoding.UTF8.GetBytes(csr);
            var encodedString = Convert.ToBase64String(bytes);
            var methodType = productInfo.ProductParameters["Domain Control Validation Method"];
            var certificateType = GetCertificateType(productInfo.ProductID);

            return new RegistrationRequest
            {
                Csr = encodedString,
                ServerSoftware = "-1", //Just default to other, user does not need to fill this in
                CertificateType = certificateType,
                Term = productInfo.ProductParameters["Term"],
                ApplicantFirstName = productInfo.ProductParameters["Applicant First Name"],
                ApplicantLastName = productInfo.ProductParameters["Applicant Last Name"],
                ApplicantEmailAddress = productInfo.ProductParameters["Applicant Email Address"],
                ApplicantPhoneNumber = productInfo.ProductParameters["Applicant Phone (+nn.nnnnnnnn)"],
                DomainControlValidation = GetDomainControlValidation(methodType, null),
                OrganizationContact = productInfo.ProductParameters["Organization Contact"],
                BusinessUnit = productInfo.ProductParameters["Business Unit"],
                ShowPrice = true, //User should not have to fill this out
                CustomFields = GetCustomFields(productInfo),
                SubjectAlternativeNames = certificateType == "2" ? GetSubjectAlternativeNames(productInfo, sans) : null,
                EvCertificateDetails = certificateType == "3" ? GetEvCertificateDetails(productInfo) : null
            };
        }

        private string GetCertificateType(string productId)
        {
            switch (productId)
            {
                case "CSC TrustedSecure Premium Certificate":
                    return "0";
                case "CSC TrustedSecure EV Certificate":
                    return "3";
                case "CSC TrustedSecure UC Certificate":
                    return "2";
                case "CSC TrustedSecure Premium Wildcard Certificate":
                    return "1";
            }

            return "-1";
        }

        public Notifications GetNotifications(EnrollmentProductInfo productInfo)
        {
            return new Notifications
            {
                Enabled = true,
                AdditionalNotificationEmails = productInfo.ProductParameters["Notification Email(s) Comma Separated"]
                    .Split(',').ToList()
            };
        }

        public RenewalRequest GetRenewalRequest(EnrollmentProductInfo productInfo, string uUId, string csr,
            Dictionary<string, string[]> sans)
        {
            var bytes = Encoding.UTF8.GetBytes(csr);
            var encodedString = Convert.ToBase64String(bytes);
            var methodType = productInfo.ProductParameters["Domain Control Validation Method"];
            var certificateType = GetCertificateType(productInfo.ProductID);

            return new RenewalRequest
            {
                Uuid = uUId,
                Csr = encodedString,
                ServerSoftware = "-1",
                CertificateType = certificateType,
                Term = productInfo.ProductParameters["Term"],
                ApplicantFirstName = productInfo.ProductParameters["Applicant First Name"],
                ApplicantLastName = productInfo.ProductParameters["Applicant Last Name"],
                ApplicantEmailAddress = productInfo.ProductParameters["Applicant Email Address"],
                ApplicantPhoneNumber = productInfo.ProductParameters["Applicant Phone (+nn.nnnnnnnn)"],
                DomainControlValidation = GetDomainControlValidation(methodType, null),
                OrganizationContact = productInfo.ProductParameters["Organization Contact"],
                BusinessUnit = productInfo.ProductParameters["Business Unit"],
                ShowPrice = true,
                SubjectAlternativeNames = certificateType == "2" ? GetSubjectAlternativeNames(productInfo, sans) : null,
                CustomFields = GetCustomFields(productInfo),
                EvCertificateDetails = certificateType == "3" ? GetEvCertificateDetails(productInfo) : null
            };
        }

        private List<SubjectAlternativeName> GetSubjectAlternativeNames(EnrollmentProductInfo productInfo,
            Dictionary<string, string[]> sans)
        {
            var subjectNameList = new List<SubjectAlternativeName>();
            var methodType = productInfo.ProductParameters["Domain Control Validation Method"];

            foreach (var v in sans["dns"])
            {
                var domainName = v;
                var san = new SubjectAlternativeName();
                san.DomainName = domainName;
                if (methodType.ToUpper() == "EMAIL")
                    san.DomainControlValidation = GetDomainControlValidation(methodType, null, domainName);
                else //it is a CNAME validation so no email is needed
                    san.DomainControlValidation = GetDomainControlValidation(methodType, "");

                subjectNameList.Add(san);
            }

            return subjectNameList;
        }

        public ReissueRequest GetReissueRequest(EnrollmentProductInfo productInfo, string uUId, string csr,
            Dictionary<string, string[]> sans)
        {
            var bytes = Encoding.UTF8.GetBytes(csr);
            var encodedString = Convert.ToBase64String(bytes);
            var methodType = productInfo.ProductParameters["Domain Control Validation Method"];
            var certificateType = GetCertificateType(productInfo.ProductID);

            return new ReissueRequest
            {
                Uuid = uUId,
                Csr = encodedString,
                ServerSoftware = "-1",
                CertificateType = GetCertificateType(productInfo.ProductID),
                Term = productInfo.ProductParameters["Term"],
                ApplicantFirstName = productInfo.ProductParameters["Applicant First Name"],
                ApplicantLastName = productInfo.ProductParameters["Applicant Last Name"],
                ApplicantEmailAddress = productInfo.ProductParameters["Applicant Email Address"],
                ApplicantPhoneNumber = productInfo.ProductParameters["Applicant Phone (+nn.nnnnnnnn)"],
                DomainControlValidation = GetDomainControlValidation(methodType, null),
                OrganizationContact = productInfo.ProductParameters["Organization Contact"],
                BusinessUnit = productInfo.ProductParameters["Business Unit"],
                ShowPrice = true,
                SubjectAlternativeNames = certificateType == "2" ? GetSubjectAlternativeNames(productInfo, sans) : null,
                CustomFields = GetCustomFields(productInfo),
                EvCertificateDetails = certificateType == "3" ? GetEvCertificateDetails(productInfo) : null
            };
        }

        private EvCertificateDetails GetEvCertificateDetails(EnrollmentProductInfo productInfo)
        {
            var evDetails = new EvCertificateDetails();
            evDetails.Country = productInfo.ProductParameters["Organization Country"];
            return evDetails;
        }

        public int MapReturnStatus(string digiCertSymStatus)
        {
            PKIConstants.Microsoft.RequestDisposition returnStatus;

            switch (digiCertSymStatus)
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

        public void SendEmail(string smtpEmailHost, string fromEmailAddress, string toAddress, string subject,
            string userId, string password, int port, List<string> bodyLines)
        {
            var emailAddresses = toAddress.Split(',');
            foreach (var emailAddress in emailAddresses)
            {
                //Send Email to Service Now with certificate attached
                var client = new SmtpClient(smtpEmailHost);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(userId,
                    password);
                client.Port = 587;
                var from = new MailAddress(fromEmailAddress,
                    "Automated Keyfactor Gateway Email",
                    Encoding.UTF8);
                var to = new MailAddress(emailAddress);
                var message = new MailMessage(from, to);
                message.BodyEncoding = Encoding.UTF8;
                message.Subject = subject;
                message.SubjectEncoding = Encoding.UTF8;
                foreach (var bodyLine in bodyLines)
                {
                    message.Body += bodyLine;
                    message.Body += Environment.NewLine;
                }

                client.Send(message);
            }

        }
    }
}