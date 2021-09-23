namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IDomainControlValidation
    {
        string MethodType { get; set; }
        string EmailAddress { get; set; }
    }
}