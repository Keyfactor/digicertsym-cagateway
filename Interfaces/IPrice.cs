namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IPrice
    {
        string Currency { get; set; }
        decimal Total { get; set; }
    }
}