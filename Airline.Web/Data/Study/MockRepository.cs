using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public class MockRepository : IRepository
    {
        public void AddDestination(Destination destination)
        {
            throw new NotImplementedException();
        }

        public bool DestinationExists(int id)
        {
            throw new NotImplementedException();
        }

        public Destination GetDestination(int id)
        {
            throw new NotImplementedException();

        }

        public IEnumerable<Destination> GetDestinations()
        {
            var destination = new List<Destination>();

           /* destination.Add(new Destination
            {
                Id = 1,
                IATA = "FAO",
                Airport = "Aeoroporto Internacional de Faro",
                City = "Faro",
                Country = "Portugal"

            });


            destination.Add(new Destination
            {
                Id = 2,
                IATA = "BCN",
                Airport = "Josep Tarradellas Barcelona-El Prat",
                City = "Barcelona",
                Country = "Spain"

            });*/

            return destination;

        }

        public void RemoveDestination(Destination destination)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAllAsync()
        {
            throw new NotImplementedException();
        }

        public void UpdateDestination(Destination destination)
        {
            throw new NotImplementedException();
        }
    }
}
