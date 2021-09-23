using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;

namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IDigiCertSymClient
    {
        Task<RegistrationResponse> SubmitRegistrationAsync(
            RegistrationRequest registerRequest);

        Task<RenewalResponse> SubmitRenewalAsync(
            RenewalRequest renewalRequest);

        Task<ReissueResponse> SubmitReissueAsync(
            ReissueRequest reissueRequest);

        Task<CertificateResponse> SubmitGetCertificateAsync(string certificateId);

        Task SubmitCertificateListRequestAsync(BlockingCollection<ICertificateResponse> bc, CancellationToken ct);

        Task<RevokeResponse> SubmitRevokeCertificateAsync(string uuId);
    }
}