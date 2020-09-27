using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class ChooseSeatFlightModel
    {
       
        public int FlightId { get; set; }

        public int FlightIdReturn { get; set; }

        public List<string> SeatIsAvailable { get; set; }

        public List<string> SeatIsAvailableReturn { get; set; }

        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Classes { get; set; }
   
        public int Class { get; set; }

        public int ClassReturn { get; set; }

        public int Seat { get; set; }

        public int SeatReturn { get; set; }

        public bool isRoundTrip { get; set; }

      
    }
}
