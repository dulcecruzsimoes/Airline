using Airline.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
    public class AirplaneRepository : GenericRepository<Airplane>, IAirplaneRepository
    {
        private readonly DataContext _context;

        public AirplaneRepository(DataContext context) : base (context)
        {
            _context = context;

        }

        public async Task<Airplane> GetAirplaneWithUserAsync(int id) 
        {

            var airplaine = await _context.Airplanes
                            .Include(d => d.User)
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();

            return airplaine;



        }
    }
}
