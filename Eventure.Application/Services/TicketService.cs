using Eventure.Domain.Absctrations;
using Eventure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventure.Domain.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        public TicketService(ITicketRepository repository)
        {
            _ticketRepository = repository;
        }

        public async Task<List<Ticket>> GetAllTickets()
        {
            return await _ticketRepository.GetAll();
        }
        public async Task<Ticket> GetTicketById(int id)
        {
            return await _ticketRepository.GetById(id);
        }
        public async Task<int> CreateTicket(Ticket ticket)
        {
            return await _ticketRepository.Create(ticket);
        }

        public async Task<int> UpdateTicket(int id, string name, string location, DateTime date, int freeSeats, decimal price)
        {
            return await _ticketRepository.Update(id, name, location, date, freeSeats, price);
        }

        public async Task<int> DeleteTicket(int id)
        {
            return await _ticketRepository.Delete(id);
        }
    }
}
