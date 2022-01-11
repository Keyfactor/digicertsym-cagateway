using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Keyfactor.AnyGateway.DigiCertSym.DigicertMPKISOAP;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IDigiCertSymClient
    {
        Task<EnrollmentResponse> SubmitEnrollmentAsync(
            EnrollmentRequest enrollmentRequest);

        Task<EnrollmentResponse> SubmitRenewalAsync(string serialNumber,
            EnrollmentRequest renewalRequest);

        Task<RevokeResponse> SubmitRevokeCertificateAsync(string serialNumber, RevokeRequest req);

        Task<GetCertificateResponse> SubmitGetCertificateAsync(string serialNumber);

        Task SubmitQueryOrderRequestAsync(BlockingCollection<CertificateSearchResultType> bc, CancellationToken ct,
            RequestManager requestManager);
    }
}