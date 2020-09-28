using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class ShowListFlightsModel
    {

        public List<Flight> Flights { get; set; }

        public List<Flight> FlightsReturn { get; set; }

        [Range(1, 2, ErrorMessage = "Please, choose one trip type")]
        public int isRoundTrip { get; set; }

        [Required(ErrorMessage ="Choose a flight")]
        public int flightId { get; set; }

        [Attributes.RequieredIfRoundTrip("isRoundTrip", "flightIdReturn")] // Caso tenha sido escolhido roundtrip, este valor não pode ser nulo
        public int flightIdReturn { get; set; }

        
    }
}
