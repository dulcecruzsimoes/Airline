using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airline.Web.Models;
using Airline.Web.Data.Repository_CRUD;

namespace Airline.Web.Controllers
{
    public class HomeController : Controller
    {

        public HomeController(IFlightRepository flightRepository)
        {
            // Actualizar o estado dos voos
            // Chamar todos os voos activos e aqueles cuja data de chegada seja menor que a data actual passá-los para Concluded
            flightRepository.UpdateFlightStatus(DateTime.Now);

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]

        public IActionResult Error404() 
        {
            return View();
        }
    }
}
