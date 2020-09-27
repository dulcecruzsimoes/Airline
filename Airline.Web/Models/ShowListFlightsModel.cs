using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class ShowListFlightsModel
    {

        public List<Flight> Flights { get; set; }

        public List<Flight> FlightsReturn { get; set; }

        public bool isRoundTrip { get; set; }

        public int flightId { get; set; }

        public int flightIdReturn { get; set; }

        
    }
}
