﻿using System.Collections.Generic;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IReissueRequest
    {
        string Uuid { get; set; }
        string Csr { get; set; }
        string CertificateType { get; set; }
        string BusinessUnit { get; set; }
        string Term { get; set; }
        string ServerSoftware { get; set; }
        string OrganizationContact { get; set; }
        DomainControlValidation DomainControlValidation { get; set; }
        Notifications Notifications { get; set; }
        bool ShowPrice { get; set; }
        List<CustomField> CustomFields { get; set; }
        string ApplicantFirstName { get; set; }
        string ApplicantLastName { get; set; }
        string ApplicantEmailAddress { get; set; }
        string ApplicantPhoneNumber { get; set; }
        EvCertificateDetails EvCertificateDetails { get; set; }
        List<SubjectAlternativeName> SubjectAlternativeNames { get; set; }
    }
}