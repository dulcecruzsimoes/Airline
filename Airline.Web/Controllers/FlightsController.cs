using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Helpers;
using Airline.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Schedule;

namespace Airline.Web.Controllers
{
    public class FlightsController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;

        public FlightsController(IUserHelper userHelper, IFlightRepository flightRepository, ITicketRepository ticketRepository)
        {
            _userHelper = userHelper;
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
        }



        public IActionResult Index()
        {

            var model = new FlightViewModel()
            {
                States = _flightRepository.GetComboStates(), // Apresentar uma combobox com os estado dos voos para posteriormente apresentar os voos na tabela
                Flights = _flightRepository.GetAll().ToList(),
                
            };

            return View(model);
          

        }

        // Obtem o Json com todos os voos que se encontram activos
        public JsonResult GetFlightState(int stateId)
        {
            if (stateId == 0)
            {
                FlightViewModel flightEmpty = new FlightViewModel() { };
                return this.Json(flightEmpty);
            }

            var flights =  _flightRepository.GetFlightsByState(stateId).ToList();

            FlightViewModel model = new FlightViewModel()
            {
                Flights = flights,
                StateId = stateId,
                States = _flightRepository.GetComboStates(),
            };


            return this.Json(model);

        }

        public IActionResult Create()
        {
            CreateFlightViewModel model = new CreateFlightViewModel()
            {
                Airplaines = _flightRepository.GetComboAirplaines(),
                Destinations = _flightRepository.GetComboDestinations(),
                States = _flightRepository.GetComboStates(),
            };

            return View(model);

        }



    }
}
