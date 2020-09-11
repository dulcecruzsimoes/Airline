using Airline.Web.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class ChangeDptEmployeeViewModel
    {
        [Required]
        public string UserId { get; set; }


        [Display(Name ="First Name")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "Current Department")]
        public string OldDepartment { get; set; }


        public int OldDepartmentDetailId { get; set; }


   
        public string NewDepartment { get; set; }


    
        public int NewDepartmentId { get; set; }


        public IEnumerable<SelectListItem> Departments { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "The field {0} is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public object BeginOldDepartment { get; set; } = DateTime.Parse("3000-01-01");

        [Attributes.DateLessThan("BeginOldDepartment")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage ="The field {0} is required")]
        public DateTime EndOldDepartment { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? BeginNewDepartment { get; set; }


        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }

        }

        
      
    }
}
