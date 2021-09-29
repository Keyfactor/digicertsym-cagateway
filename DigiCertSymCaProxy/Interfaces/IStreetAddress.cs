namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IStreetAddress
    {
        string Id { get; set; }
        bool Mandatory { get; set; }
        string Type { get; set; }
        string Value { get; set; }
    }
}