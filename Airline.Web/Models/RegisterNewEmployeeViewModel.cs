using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class RegisterNewEmployeeViewModel : RegisterNewUserViewModel
    {

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

        [Required]
        [Display(Name = "Tax Number")]
        public string TaxNumber { get; set; }

        [Required]
        [Display(Name = "Social Security Number")]
        public string SocialSecurityNumber { get; set; }
    }
}
