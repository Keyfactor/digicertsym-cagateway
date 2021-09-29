namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IOtherName
    {
        string Id { get; set; }
        bool Mandatory { get; set; }
        string Type { get; set; }
        string Value { get; set; }
    }
}