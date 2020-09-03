using Airline.Web.Data.Validations;
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

        public string UserId { get; set; }

        public string FirstName { get; set; }


        public string LastName { get; set; }



        public string OldDepartment { get; set; }

        [Required]
        public int OldDepartmentDetailId { get; set; }


        [Compare(nameof(OldDepartment), ErrorMessage = "The new department must be different from the previous")]
        public string NewDepartment { get; set; }


        [Display(Name = "Department")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a department")]
        public int NewDepartmentId { get; set; }


        public IEnumerable<SelectListItem> Departments { get; set; }


      
        public DateTime? BeginOldDepartment { get; set; }



        public DateTime? EndOldDepartment { get; set; }



        [DateBiggerThan("EndOldDepartment", ErrorMessage = "Inavlid Date")]
        [Display(Name = "Begin Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
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
