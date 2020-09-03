using Airline.Web.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public interface IRepository
    {

        void AddDestination(Destination destination);


        bool DestinationExists(int id);


        Destination GetDestination(int id);


        IEnumerable<Destination> GetDestinations();


        void RemoveDestination(Destination destination);


        Task<bool> SaveAllAsync();


        void UpdateDestination(Destination destination);
    }
}