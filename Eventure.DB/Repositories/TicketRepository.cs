using Eventure.DB.Entities;
using Eventure.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using Eventure.Domain.Absctrations;
using AutoMapper;

namespace Eventure.DB.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        public TicketRepository(AppDbContext context, IMapper mapper)
        {
            _appDbContext = context;
            _mapper = mapper;
        }
        public async Task<int> Create(Ticket ticket)
        {

            var ticketEntity = _mapper.Map<TicketEntity>(ticket);
            await _appDbContext._tickets.AddAsync(ticketEntity);
            await _appDbContext.SaveChangesAsync();
            return ticketEntity.Id;
        }

        public async Task<int> Delete(int id)
        {
            await _appDbContext._tickets
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();
            return id;
        }

        public async Task<List<Ticket>> GetAll()
        {
            var ticketsEntities = await _appDbContext._tickets
                .AsNoTracking()
                .ToListAsync();

            var tickets = ticketsEntities
                .Select(_mapper.Map<Ticket>)
                .ToList();

            return tickets;
        }


        public async Task<Ticket> GetById(int id)
        {
            var ticketEntity = await _appDbContext._tickets
                .FirstOrDefaultAsync(t => t.Id == id); 
            var ticket = _mapper.Map<Ticket>(ticketEntity);
            return ticket;
        }

        public async Task<int> Update(int id, string name, string location, DateTime date, int freeSeats, decimal price)
        {
            await _appDbContext._tickets
                .Where(t => t.Id == id)
                .ExecuteUpdateAsync(s =>
                s
                .SetProperty(t => t.Name, t => name)
                .SetProperty(t => t.Location, t => location)
                .SetProperty(t => t.Date, t => date)
                .SetProperty(t => t.FreeSeats, t => freeSeats)
                .SetProperty(t => t.Price, t => price)
                );
            return id;
        }
    }
}
