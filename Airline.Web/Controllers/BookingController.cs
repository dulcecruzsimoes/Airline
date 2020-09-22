using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Airline.Web.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            //List<district> list = new List<district>();
            //list.Add(new district
            //{
            //    name = "Faro"
            //});
            //list.Add(new district
            //{
            //    name = "Lisboa"
            //});
            //list.Add(new district
            //{
            //    name = "Setubal"
            //});
            //list.Add(new district
            //{
            //    name = "Porto"
            //});

            //ViewBag.districtData = list;
            //ViewBag.usmap = GetUSMap();
            return View();
          
        }
        public IActionResult Movie()
        {

         
            return View();
        }

            public object GetUSMap()
        {
            string allText = System.IO.File.ReadAllText("./wwwroot/scripts/MapsData/Portugal.json");
            return JsonConvert.DeserializeObject(allText);
        }
    }

    public class district 
    {
        public string name { get; set; }


    }
}
