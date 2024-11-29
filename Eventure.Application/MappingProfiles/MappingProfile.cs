using AutoMapper;
using Eventure.Application.Contracts;
using Eventure.DB.Entities;
using Eventure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventure.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Ticket, TicketRequest>();
            CreateMap<TicketRequest, Ticket>();
            CreateMap<TicketResponse, Ticket>();
            CreateMap<Ticket, TicketResponse>();
            CreateMap<TicketEntity, Ticket>();
            CreateMap<Ticket, TicketEntity>();
        }
    }
}
