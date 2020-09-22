using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Entities
{
    public class DateTimeValue
    {

        [Required(ErrorMessage = "Please enter the value")]
        public DateTime? value { get; set; }

    }
}
