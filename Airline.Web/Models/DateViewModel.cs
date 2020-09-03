using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class DateViewModel
    {

        public DateTime? Value { get; set; }

        public string ErrorMessage { get; set; }
    } 
}
