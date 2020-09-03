using Airline.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class EmployeeViewModel
    {
        public string UserId { get; set; }


        [Required(ErrorMessage = "The field {0} is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        public string TaxNumber { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        public string SocialSecurityNumber { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        public string PhoneNumber { get; set; }


        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        public string Department { get; set; }

        
        [Display(Name = "Department")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a department")]
        public int DepartmentId { get; set; }


        public IEnumerable<SelectListItem> Departments { get; set; }


        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public DateTime StartDate { get; set; }


        [Display(Name = "Close Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public DateTime? CloseDate { get; set; }


        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city")]
        public int CityId { get; set; }


        
        public string City { get; set; }


        public IEnumerable<SelectListItem> Cities { get; set; }


        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country")]
        public int CountryId { get; set; }


        public IEnumerable<SelectListItem> Countries { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

    }
}
