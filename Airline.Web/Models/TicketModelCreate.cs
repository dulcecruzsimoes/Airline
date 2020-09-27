using Airline.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class TicketModelCreate
    {
        [Required(ErrorMessage = "Select a flight!")]
        public int FlightId { get; set; }

        [Required(ErrorMessage ="Select a client!")]
        public string UserEmail { get; set; }

        public string FullName { get; set; }

        public List<User> Users { get; set; }

        public List<string> SeatIsAvailable { get; set; }

        //SelectListItem é a combobox (Vai renderizar)
        public IEnumerable<SelectListItem> Classes { get; set; }


        public string ClassName { get; set; }


        public string From { get; set; }

        public string To { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        [Required(ErrorMessage = "Select a class!")]
        [Range(1, 2, ErrorMessage = "Select a class!")]
        public int Class { get; set; }

        [Required(ErrorMessage = "Pick a seat!")]
        [Range(1, 56, ErrorMessage = "Pick a seat!")]
        public int Seat { get; set; }

    }
}
