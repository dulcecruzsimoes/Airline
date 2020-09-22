using Airline.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class AirplaneViewModel : Airplane
    {
        [Display(Name ="Image")]
        public IFormFile ImageFile { get; set; }
    }
}
