namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ICertificateSearchRequest
    {
        string SeatId { get; set; }
        string CommonName { get; set; }
        string Email { get; set; }
        string IssuingCa { get; set; }
        string ProfileId { get; set; }
        string SerialNumber { get; set; }
        int StartIndex { get; set; }
        string Status { get; set; }
        string ValidFrom { get; set; }
        string ValidTo { get; set; }
    }
}