using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public interface IDestinationRepository : IGenericRepository<Destination>
    {

        IQueryable GetAllWithUsers();
    }
}
