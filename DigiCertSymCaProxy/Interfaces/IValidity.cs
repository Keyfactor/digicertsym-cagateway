namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IValidity
    {
        int Duration { get; set; }
        string Unit { get; set; }
    }
}