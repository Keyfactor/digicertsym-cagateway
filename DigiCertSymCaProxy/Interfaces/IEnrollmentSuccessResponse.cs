namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IEnrollmentSuccessResponse
    {
        string SerialNumber { get; set; }
        string DeliveryFormat { get; set; }
        string Certificate { get; set; }
        string Pkcs12Password { get; set; }
        string Status { get; set; }
    }
}