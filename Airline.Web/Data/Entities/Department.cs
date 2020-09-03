using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Entities
{
    public class Department : IEntity
    {


        public int Id { get; set; }



        [Required]
        [Display(Name = "Department Name")]        
        public string Name { get; set; }


        // Mais prático para fazer as querys
       public IEnumerable<DepartmentDetail> Items { get; set; }




       
    }
}
