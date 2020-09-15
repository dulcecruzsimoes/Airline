using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class DestinationViewModel
    {
        public int Id { get; set; }



        //Se a cidade só tiver este aeroporto o campo não é necessário (Esta validação será realizada do lado do cliente)
        [Display(Name = "Airport")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} character long.")]
        public string Airport { get; set; }


        //Aceitar apenas 3 Letras
        [Display(Name = "IATA")]
        [Required(ErrorMessage = "The filed {0} is required")]
        [StringLength(3, ErrorMessage = "The field {0} need to contain {1} characters long", MinimumLength = 3)]
        public string IATA { get; set; }



        public string UserId { get; set; }


        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city")]
        public int CityId { get; set; }


      
        public string City { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }


        
        public string Country { get; set; }

        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country")]
        public int CountryId { get; set; }


        public IEnumerable<SelectListItem> Countries { get; set; }



    }
}
