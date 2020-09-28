using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airline.Web.Models;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Data.Entities;
using Airline.Web.Data;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using Airline.Web.Helpers;

namespace Airline.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IDestinationRepository _destinationRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public HomeController(IFlightRepository flightRepository, ITicketRepository ticketRepository, IDestinationRepository destinationRepository, IUserHelper userHelper, IMailHelper mailHelper)
        {
            // Actualizar o estado dos voos
            // Chamar todos os voos activos e aqueles cuja data de chegada seja menor que a data actual passá-los para Concluded
            flightRepository.UpdateFlightStatus(DateTime.Now);
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
            _destinationRepository = destinationRepository;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SearchFlightModel model)
        {
            if (ModelState.IsValid)
            {
                var destinationFrom = await _destinationRepository.GetDestinationByNameAsync(model.From);
                //Confirmar a existência das cidades
                if (destinationFrom == null)
                {
                    this.ModelState.AddModelError(string.Empty, "Destination From does not exist!");
                    return View(model);
                }

                var destinationTo = await _destinationRepository.GetDestinationByNameAsync(model.To);
                //Confirmar a existência das cidades
                if (destinationTo == null)
                {
                    this.ModelState.AddModelError(string.Empty, "Destination To does not exist!");
                    return View(model);
                }

                ShowListFlightsModel modelView = new ShowListFlightsModel();
                modelView.Flights = _flightRepository.GetFlightsFromToAndDeparture(model.From, model.To, model.Departure);

                if (modelView.Flights.Count == 0)
                {
                    this.ModelState.AddModelError(string.Empty, "There are no flights from the selected airport!");
                    return View(model);
                }

                modelView.isRoundTrip = 2; // One-Way

                if (model.Trip == 1) // Roundtrip
                {
                    modelView.FlightsReturn = _flightRepository.GetFlightsFromToAndDeparture(model.To, model.From, model.Return);
                    modelView.isRoundTrip = 1;

                    if (modelView.FlightsReturn.Count == 0)
                    {

                        modelView.isRoundTrip = 1;
                        this.ModelState.AddModelError(string.Empty, "There are no flights from the return airport!");
                        return View(model);
                    }
                }
                // Redireccionar para o booking (levar o modelo)
                TempData.Put("FlightsList", modelView);
                return RedirectToAction("ViewFlights", "Home");
            }

            return View(model);
        }



        public IActionResult ViewFlights()
        {
            // Agarrar o modelo

            var data = TempData.Get<ShowListFlightsModel>("FlightsList");

            if (data == null)
            {
                return NotFound();
            }

            ShowListFlightsModel model = new ShowListFlightsModel();

            model.flightId = data.flightId;
            model.flightIdReturn = data.flightIdReturn;
            model.Flights = data.Flights;
            model.FlightsReturn = data.FlightsReturn;
            model.isRoundTrip = data.isRoundTrip;

            return View(model);
        }

        [HttpPost]
        public IActionResult ViewFlights(ShowListFlightsModel model)
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

                foreach (var item in ticketsList)
                {
                    int index = (item.Seat) - 1;

                    TicketsClassArray[index] = "occupied";

                }

                ////=====================Flight Return================

                if (model.isRoundTrip == 1)
                {
                    var flightReturn = _flightRepository.GetFlight(model.flightIdReturn);

                    if (flightReturn == null)
                    {
                        return NotFound();
                    }

                    // Criar a lista com as classes para passar para a view
                    string[] TicketsClassArrayReturn = new string[64];

                    List<int> TicketsClassListReturn = new List<int>(); // 8 Executiva + 56 Económica = 64 lugares

                    // Obter a lista de bilhetes existentes para o voo de regresso
                    var ticketsListReturn = _ticketRepository.FlightTickets(flightReturn.Id);


                    foreach (var item in ticketsListReturn)
                    {
                        int index = (item.Seat) - 1;

                        TicketsClassArrayReturn[index] = "occupied";
                    }

                    chooseSeatFlight.FlightIdReturn = flightReturn.Id;
                    chooseSeatFlight.SeatIsAvailableReturn = TicketsClassArrayReturn.ToList();
                }

                //======================Fim Flight Return===================================

                chooseSeatFlight.FlightId = flight.Id;
                chooseSeatFlight.Classes = list;
                chooseSeatFlight.SeatIsAvailable = TicketsClassArray.ToList();


                // Redireccionar para o booking (levar o modelo)
                TempData.Put("Booking", chooseSeatFlight);
                return RedirectToAction("Booking", "Home");


            }

            return View(model);

        }



        public IActionResult Booking()
        {
            // Agarrar o modelo

            var data = TempData.Get<ChooseSeatFlightModel>("Booking");

            if (data == null)
            {
                return NotFound();
            }

            ChooseSeatFlightModel model = new ChooseSeatFlightModel();

            model.Classes = data.Classes;
            model.FlightId = data.FlightId;
            model.FlightIdReturn = data.FlightIdReturn;
            model.SeatIsAvailable = data.SeatIsAvailable;
            model.SeatIsAvailableReturn = data.SeatIsAvailableReturn;
            model.isRoundTrip = data.isRoundTrip;

            return View(model);

        }

        [HttpPost]
        public IActionResult Booking(ChooseSeatFlightModel model)
        {

            if (model == null)
            {
                return NotFound();
            }

            // Redireccionar para o booking (levar o modelo)
            TempData.Put("BookingRetun", model);

            if (model.isRoundTrip == 1)
            {
                return RedirectToAction("Booking", "Home");
            }

            return RedirectToAction("TicketNew", "Home");
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



        [Authorize]
        public async Task<IActionResult> TicketNew()
        {
            // Agarrar o modelo

            var data = TempData.Get<ChooseSeatFlightModel>("BookingRetun");

            if (data == null)
            {
                return NotFound();
            }

            NewTicketModel model = new NewTicketModel();

            string email = this.User.Identity.Name;

            User user = await _userHelper.GetUserByEmailAsync(email);

            Flight flight = await _flightRepository.GetFlightWithObjectsAsync(data.FlightId);

            Destination From = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(flight.From.Id);

            Destination To = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(flight.To.Id);

            model.FullName = user.FullName;
            model.UserEmail = user.Email;

            //======== Bilhete de Ida ========//
            model.FlightId = flight.Id;
            model.From = From.City.Name;
            model.To = To.City.Name;
            model.Date = flight.Departure.ToShortDateString();
            model.Time = flight.Departure.ToShortTimeString();
            model.Seat = data.Seat.ToString();
            if (data.Class == 1)
            {
                model.ClassName = "Economic";
            }

            else if (data.Class == 2)
            {
                model.ClassName = "Business";
            }

            if (data.isRoundTrip == 1) // Ida e Volta
            {
                Flight flightReturn = await _flightRepository.GetFlightWithObjectsAsync(data.FlightIdReturn);
                //======== Bilhete de Volta ========//
                model.FlightIdReturn = flightReturn.Id;
                model.From = To.City.Name;
                model.To = From.City.Name;
                model.DateReturn = flightReturn.Departure.ToShortDateString();
                model.TimeReturn = flightReturn.Departure.ToShortTimeString();
                model.SeatReturn = data.SeatReturn.ToString();
                if (data.ClassReturn == 1)
                {
                    model.ClassName = "Economic";
                }

                else if (data.Class == 2)
                {
                    model.ClassName = "Business";
                }

            }

            return View(model);

        }


        [HttpPost]
        public async Task<IActionResult> TicketNew(NewTicketModel model)
        {
            // É aqui que vou inserir os bilhetes na base de dados

            User user = await _userHelper.GetUserByEmailAsync(model.UserEmail);

            // ===================== Bilhete de Ida ===========================//
            Ticket ticket = new Ticket();
            Flight flight = _flightRepository.GetFlight(model.FlightId);
            ticket.Seat = Convert.ToInt32(model.Seat);
            ticket.User = user;
            ticket.Flight = flight;
            ticket.Class = model.ClassName;

            try
            {
                await _ticketRepository.CreateAsync(ticket);// Ao usar o create grava logo

                _mailHelper.SendMail(user.Email, "Ticket", $"<h1>Ticket Confirmation</h1>" +
                    $"Your ticket information, " +
                    $"Flight: {ticket.Flight.Id}, " +
                    $"Class: {ticket.Class}, " +
                    $"Date: {ticket.Seat}, " +
                    $"Thanks for flying with us!");

                ViewBag.Message = "Your ticket information was sent to email!";
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                return View(model);
            }       

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
