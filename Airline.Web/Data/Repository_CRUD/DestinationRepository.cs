using Airline.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public class DestinationRepository : GenericRepository<Destination>, IDestinationRepository
    {
        private readonly DataContext _context;

        public DestinationRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Destinations.Include(d => d.User);
        }
    }
}
