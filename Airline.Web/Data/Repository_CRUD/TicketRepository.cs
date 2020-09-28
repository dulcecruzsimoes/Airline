using Airline.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
    public class TicketRepository: GenericRepository<Ticket>, ITicketRepository
    {
        private DataContext _context { get; set; }

        public TicketRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public List<Ticket> FlightTickets(int flightId)
        {
            return _context.Tickets
                .Include(c => c.Flight)
                .Where(x=>x.Flight.Id == flightId)               
                .ToList();
        }

        public List<Ticket> FlightTicketsByUser(string email)
        {
            return _context.Tickets
                .Include(c => c.Flight)
                .ThenInclude(c => c.From)
                .Include(c=> c.Flight)
                .ThenInclude(c => c.To)
                .Include (c => c.User)
                .Where(x => x.User.Email == email)
                .ToList();
        }
    }
}
