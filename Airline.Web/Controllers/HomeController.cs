using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airline.Web.Models;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Data.Entities;

namespace Airline.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;

        public HomeController(IFlightRepository flightRepository, ITicketRepository ticketRepository)
        {
            // Actualizar o estado dos voos
            // Chamar todos os voos activos e aqueles cuja data de chegada seja menor que a data actual passá-los para Concluded
            flightRepository.UpdateFlightStatus(DateTime.Now);
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
        }

        public IActionResult Index()
        {

            return View();
        }


        [HttpPost]
        public IActionResult ViewFlights(SearchFlightModel model)
        {
            if (ModelState.IsValid)
            {
                ShowListFlightsModel modelView = new ShowListFlightsModel();
                modelView.Flights = _flightRepository.GetFlightsFromToAndDeparture(model.To, model.From, model.Departure);
                modelView.isRoundTrip = false;

                if (model.Trip == 1) // Roundtrip
                {
                    modelView.FlightsReturn = _flightRepository.GetFlightsFromToAndDeparture(model.From, model.To, model.Return);
                    modelView.isRoundTrip = true;
                }


                return View(modelView);
            }


            ViewBag.Message = "Please, confirm data!";
            return View();
        }



        [HttpPost]
        public IActionResult Booking (ShowListFlightsModel model)
        {
            if (ModelState.IsValid)
            {
                ChooseSeatFlightModel chooseSeatFlight = new ChooseSeatFlightModel();

                // 1º: Obter a lista das classes
                var list = _flightRepository.GetComboClasses(); // Obter as classes

                //2ª O cliente escolheu ida e volta?

                chooseSeatFlight.isRoundTrip = model.isRoundTrip;


                // 3º: Verificar a existência do voo de ida
                var flight = _flightRepository.GetFlight(model.flightId);

         

                if (flight == null)
                {
                    return this.RedirectToAction("Index");
                }


                //4º: Obter a lista de bilhetes existentes para o voo de ida
                var ticketsList = _ticketRepository.FlightTickets(model.flightId);


                // 4º Criar a lista com as classes para passar para a view
                string[] TicketsClassArray = new string[64];

                List<int> TicketsClassList = new List<int>(); // 8 Executiva + 56 Económica = 64 lugares



                foreach (var item in ticketsList)
                {
                    int index = (item.Seat) - 1;

                    TicketsClassArray[index] = "occupied";

                }

                chooseSeatFlight.FlightId = flight.Id;
                chooseSeatFlight.Classes = list;
                chooseSeatFlight.SeatIsAvailable = TicketsClassArray.ToList();               
             
                return View(chooseSeatFlight);

            }


            ViewBag.Message = "Please, confirm data!";
            return View();

        }


        [HttpPost]
        public IActionResult BookingReturn(ChooseSeatFlightModel model)
        {

            // Verificar a existência do voo de regresso
            var flight = _flightRepository.GetFlight(model.FlightIdReturn);



            if (flight == null)
            {
                return this.RedirectToAction("Index");
            }

            var list = _flightRepository.GetComboClasses(); // Obter as classes

            // Obter a lista de bilhetes existentes para o voo de regresso
            var ticketsList = _ticketRepository.FlightTickets(model.FlightIdReturn);


            // Criar a lista com as classes para passar para a view
            string[] TicketsClassArray = new string[64];

            List<int> TicketsClassList = new List<int>(); // 8 Executiva + 56 Económica = 64 lugares



            foreach (var item in ticketsList)
            {
                int index = (item.Seat) - 1;

                TicketsClassArray[index] = "occupied";

            }

            model.FlightIdReturn = flight.Id;
            model.Classes = list;
            model.SeatIsAvailableReturn = TicketsClassArray.ToList();

            return View(model);
        }


        [HttpPost]
        public IActionResult LoginForTicket() //////Fiquei Aqui!!//////////////////////--> 
        {

            return View(); 
        }

        public IActionResult Flights() 
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
