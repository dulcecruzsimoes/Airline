using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class CreateTicketViewModel
    {

        
        [Required(ErrorMessage = "The field {0} is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage ="The field {0} is required")]
        public int FlightId { get; set; }


        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Class { get; set; }

        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Users { get; set; }
    }
}
