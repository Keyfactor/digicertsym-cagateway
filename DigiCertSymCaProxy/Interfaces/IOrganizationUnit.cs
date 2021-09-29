namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IOrganizationUnit
    {
        string Id { get; set; }
        bool Mandatory { get; set; }
        string Type { get; set; }
        string Value { get; set; }
    }
}