namespace Eventure.Application.Contracts
{
    public record class TicketResponse(int id, string name, string location, DateTime date, int freeSeats, decimal price);
}
