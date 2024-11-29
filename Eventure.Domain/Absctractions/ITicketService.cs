using Eventure.Domain.Models;

namespace Eventure.Domain.Absctrations
{
    public interface ITicketService
    {
        Task<int> CreateTicket(Ticket ticket);
        Task<int> DeleteTicket(int id);
        Task<List<Ticket>> GetAllTickets();
        Task<Ticket> GetTicketById(int id);
        Task<int> UpdateTicket(int id, string name, string location, DateTime date, int freeSeats, decimal price);
    }
}