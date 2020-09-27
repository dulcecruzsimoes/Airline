using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        List<Ticket> FlightTickets(int flightId);
    }
}
