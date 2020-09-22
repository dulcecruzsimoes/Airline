using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
    public interface IAirplaneRepository : IGenericRepository<Airplane>
    {

        Task<Airplane> GetAirplaneWithUserAsync(int id);
    }
}
