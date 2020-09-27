using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class SearchFlightModel
    {
       [Required (ErrorMessage = "The field {0} is required")]
        public string From { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Attributes.IsDifferentString("From", "To")]
        public string To { get; set; }
             

        [Range(1, 2, ErrorMessage = "Please, choose one trip type")]
        public int Trip { get; set; }


        [Display(Name = "Departure")]
        [Attributes.DateAfterNow()]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Departure { get; set; }


        [Display(Name = "Return")]
        [Attributes.DateAfterNow()]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        //Data de chegada deve ser depois data de partida
        [Attributes.DateAfterThan("Departure", "Return")]
        public DateTime Return { get; set; }

   
    }
}
