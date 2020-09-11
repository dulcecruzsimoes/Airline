using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Entities
{
    public class DepartmentDetail : IEntity
    {
        public int Id { get; set; }

        [Required]
        public User User { get; set; }


        [Required]
        public Department Department { get; set; }


        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }



        [Display(Name = "Close Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? CloseDate { get; set; }


    }
}
