using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public interface IDestinationRepository : IGenericRepository<Destination>
    {

        List<Destination> GetAllWithUsers();

        List<Destination> GetAllWithUsersAndCountryAndCity();

        Task<Destination> GetDestinationWithUserCityAndCoutryAsync(int id);

        Task<Destination> GetDestinationByIATAAsync(string iata);
    }
}
