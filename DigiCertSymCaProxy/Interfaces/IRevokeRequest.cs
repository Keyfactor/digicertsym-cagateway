namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IRevokeRequest
    {
        string RevocationReason { get; set; }
    }
}