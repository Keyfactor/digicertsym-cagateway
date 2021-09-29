namespace Keyfactor.AnyGateway.DigiCertSym.Interfaces
{
    public interface ISeat
    {
        string Email { get; set; }
        string SeatId { get; set; }
        string SeatName { get; set; }
    }
}