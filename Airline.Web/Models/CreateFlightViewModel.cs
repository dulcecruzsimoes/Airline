using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class CreateFlightViewModel
    {

    
        public int FromId { get; set; }


        public int ToId { get; set; }


        public DateTime Departure { get; set; }


        public DateTime Arrival { get; set; }


        public int AirplaineId { get; set; }


        public int StateId { get; set; }


        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Airplaines { get; set; }



        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Destinations { get; set; }



        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> States { get; set; }
    }
}
