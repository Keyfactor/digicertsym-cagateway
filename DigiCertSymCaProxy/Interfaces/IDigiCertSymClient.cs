using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IDigiCertSymClient
    {
        Task<EnrollmentResponse> SubmitEnrollmentAsync(
            EnrollmentRequest enrollmentRequest);

        Task<EnrollmentResponse> SubmitRenewalAsync(string serialNumber,
            EnrollmentRequest renewalRequest);

        Task<RevokeResponse> SubmitRevokeCertificateAsync(string serialNumber);

        Task<GetCertificateResponse> SubmitGetCertificateAsync(string serialNumber);
    }
}