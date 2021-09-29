namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IIpAddress
    {
        string Id { get; set; }
        bool Mandatory { get; set; }
        string Type { get; set; }
        string Value { get; set; }
    }
}