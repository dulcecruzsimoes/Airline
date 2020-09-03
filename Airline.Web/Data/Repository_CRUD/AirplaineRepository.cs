using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
    public class AirplaineRepository : GenericRepository<Airplaine>, IAirplaineRepository
    {

        public AirplaineRepository(DataContext context) : base (context)
        {

        }
    }
}
