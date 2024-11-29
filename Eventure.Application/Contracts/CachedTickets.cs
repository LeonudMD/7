using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventure.Application.Contracts
{
    public class CachedTickets
    {
        public required List<TicketResponse> Tickets { get; set; }
        public bool IsStale { get; set; }
    }
}
