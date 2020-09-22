using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class CreateFlightViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must select a destination from")]
        public int From { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must select a destination to")]
        [Attributes.IsDifferent("From", "To")]
        // Destino deve ser diferente da partida
        public int To { get; set; }


        [Display(Name = "Departure")]
        [Attributes.DateAfterNow()]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]        
        public DateTime Departure { get; set; }

        
        
        [Display(Name = "Arrival")]
        [Attributes.DateAfterNow()]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        //Data de chegada deve ser depois data de partida
        [Attributes.DateAfterThan("Departure", "Arrival")]
        public DateTime Arrival { get; set; }



        [Range(1, int.MaxValue, ErrorMessage = "You must select a airplane")]
        [Display(Name = "Airplane")]
        public int AirplaneId { get; set; }

        [Range(1, 2, ErrorMessage = "Please, choose one of the states: active or canceled")]
        [Display(Name = "Status")]
        public int StatusId { get; set; }


        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Airplanes { get; set; }



        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Destinations { get; set; }



        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Status { get; set; }

        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Tickets { get; set; }
    }
}
