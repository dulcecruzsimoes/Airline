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

        public List<Destination> GetAllWithUsers()
        {
            return _context.Destinations.Include(d => d.User).ToList();
        }

        public List<Destination> GetAllWithUsersAndCountryAndCity()
        {
            return _context.Destinations
                .Include(d => d.User)
                .Include(d => d.Country)
                .Include(d => d.City)
                .ToList();
        }

        public async Task<Destination> GetDestinationWithUserCityAndCoutryAsync(int id) 
        {

            var destination = await _context.Destinations
                                .Include(d => d.User)
                                .Include(d => d.Country)
                                .Include(d => d.City)
                                .Where(x => x.Id == id)
                                .FirstOrDefaultAsync();

            return destination;
        
        }
    }
}
