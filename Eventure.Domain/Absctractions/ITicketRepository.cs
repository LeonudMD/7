using Eventure.Domain.Models;

namespace Eventure.Domain.Absctrations
{
    public interface ITicketRepository
    {
        Task<int> Create(Ticket ticket);
        Task<int> Delete(int id);
        Task<List<Ticket>> GetAll();
        Task<Ticket> GetById(int id);
        Task<int> Update(int id, string name, string location, DateTime date, int freeSeats, decimal price);
    }
}