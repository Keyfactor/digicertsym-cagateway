namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface IErrorResponse
    {
        string Code { get; set; }
        string Message { get; set; }
        string Field { get; set; }
    }
}