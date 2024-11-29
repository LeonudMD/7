using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventure.Application.Contracts
{
    public class CachedTicket
    {
        public required TicketResponse Ticket { get; set; }
        public bool IsStale { get; set; }
    }
}
