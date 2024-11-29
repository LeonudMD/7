using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventure.DB.Entities
{
    public class TicketEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int FreeSeats { get; set; }
        public decimal Price { get; set; }
    }
}
