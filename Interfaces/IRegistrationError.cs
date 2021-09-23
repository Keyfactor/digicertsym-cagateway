namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    internal interface IRegistrationError
    {
        string Code { get; set; }
        string Description { get; set; }
        string Value { get; set; }
    }
}