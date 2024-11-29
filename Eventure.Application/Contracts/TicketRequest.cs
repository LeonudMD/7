namespace Eventure.Application.Contracts
{
    public record class TicketRequest(string name, string location, DateTime date, int freeSeats, decimal price);
}
