using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Entities
{
    public class User : IdentityUser
    {

        [Required(ErrorMessage = "The field {0} is required.")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "The field {0} is required.")]
        public string LastName { get; set; }


        
        [Required (ErrorMessage = "The field {0} is required.")]
        public string TaxNumber { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        public string SocialSecurityNumber { get; set; }


        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Address { get; set; }


        public int CityId { get; set; }


        public City City { get; set; }


        [Display(Name = "Full Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        public bool isActive { get; set; }
    }
}
