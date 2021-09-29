using System;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ICertificateDetails
    {
        Profile Profile { get; set; }
        Seat Seat { get; set; }
        Account Account { get; set; }
        string Certificate { get; set; }
        string CommonName { get; set; }
        string Status { get; set; }
        string SerialNumber { get; set; }
        DateTime ValidFrom { get; set; }
        DateTime ValidTo { get; set; }
        bool IsKeyEscrowed { get; set; }
        string EnrollmentNotes { get; set; }
    }
}