﻿using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public class Status : IEntity
    {
        public int Id { get; set; }


        [Required(ErrorMessage ="The field {0} is required")]
        public string StatusName { get; set; }
    }
}
