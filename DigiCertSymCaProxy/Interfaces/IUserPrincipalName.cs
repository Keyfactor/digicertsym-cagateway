namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IUserPrincipalName
    {
        string Id { get; set; }
        bool Mandatory { get; set; }
        string Type { get; set; }
        string Value { get; set; }
    }
}