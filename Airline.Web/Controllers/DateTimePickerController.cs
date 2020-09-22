using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Web.Controllers
{
    public class DateTimePickerController : Controller
    {
        DateTimeValue valueObject = new DateTimeValue();

        public IActionResult DateTimePickerFor()
        {
            valueObject.value = new DateTime(2018, 03, 03);
            return View(valueObject);
        }
        [HttpPost]
        public IActionResult DateTimePickerFor(DateTimeValue model)
        {
            //posted value is obtained from the model
            valueObject.value = model.value;
            return View(valueObject);
        }
    }
}
