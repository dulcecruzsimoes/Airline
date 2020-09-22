using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data;
using Airline.Web.Data.Entities;
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
        private readonly IAirplaneRepository _airplaneRepository;
        private readonly IDestinationRepository _destinationRepository;
        private readonly DataContext _context;
        private readonly IMailHelper _mailHelper;

        public FlightsController(IUserHelper userHelper, IFlightRepository flightRepository, ITicketRepository ticketRepository, IAirplaneRepository airplaneRepository, 
            IDestinationRepository destinationRepository, DataContext context, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
            _airplaneRepository = airplaneRepository;
            _destinationRepository = destinationRepository;
            _context = context;
            _mailHelper = mailHelper;

            //Actualizar o estado dos voos
            flightRepository.UpdateFlightStatus(DateTime.Now);
        }



        public async Task<IActionResult> Index()
        {

            var model = new FlightViewModel()
            {
                States = _flightRepository.GetComboStatus(), // Apresentar uma combobox com os estado dos voos para posteriormente apresentar os voos na tabela
                Flights = _flightRepository.GetAllWithObjects(),
                
            };

            foreach (var item in model.Flights)
            {
                var destinationFrom = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(item.From.Id);

                var destinationTo = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(item.To.Id);

                item.From = destinationFrom;

                item.To = destinationTo;
            }

            return View(model);
          

        }

        public IActionResult Create()
        {
            CreateFlightViewModel model = new CreateFlightViewModel()
            {
                Airplanes = _flightRepository.GetComboAirplanes(),
                Destinations = _flightRepository.GetComboDestinations(),
                Status = _flightRepository.GetComboStatus(),
                StatusId = 1,
            };
           

            ViewBag.minDate = DateTime.Now;
            ViewBag.format = "dd/MM/yyyy HH:mm";
       
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFlightViewModel model)
        {
            if (ModelState.IsValid)
            {
                var airplane = await _airplaneRepository.GetAirplaneWithUserAsync(model.AirplaneId);

                if (airplane == null)
                {
                    return NotFound();
                }

                // Verificar a disponibilidade do avião ( É visto na tebela dos voos - Enviando o id do avião)
                bool isAvailable = _flightRepository.AirplaneIsAvailable(model.AirplaneId, model.Departure, model.Arrival);

                // Obter o state active
                var status = _context.Status.Where(x => x.StatusName == "Active").FirstOrDefault();

                var to = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(model.To);
                var from = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(model.From);

                if (isAvailable)
                {
                    Flight flight = new Flight()
                    {
                        From =  from,
                        To = to,
                        Departure = model.Departure,
                        Arrival = model.Arrival,
                        Airplane = airplane,
                        Status = status,
                        Business = airplane.BusinessSeats,
                        Economic = airplane.EconomySeats,
             
                    };

                    try
                    {
                        
                        await _flightRepository.CreateAsync(flight);
                        return RedirectToAction(nameof(Index));
                    }

                    catch (Exception ex)
                    {                       
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                        GetCombos(model);
                        ViewBag.minDate = DateTime.Now;
                        ViewBag.format = "dd/MM/yyyy HH:mm";
                        ViewBag.departure = model.Departure;
                        ViewBag.arrival = model.Arrival;

                        return View(model);                        
                    }
                }

                GetCombos(model);
                ModelState.AddModelError(string.Empty, "Airplane isn't available. Choose another!");
                return View(model);
            }


            GetCombos(model);
            ViewBag.minDate = DateTime.Now;
            ViewBag.format = "dd/MM/yyyy HH:mm";
            return View(model);
        }

        public void GetCombos(CreateFlightViewModel model) 
        {
            model.Airplanes = _flightRepository.GetComboAirplanes();
            model.Destinations = _flightRepository.GetComboDestinations();
            model.Status = _flightRepository.GetComboStatus();
        }

  

        // GET: Flight/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetFlightWithObjectsAsync(id.Value); // .Value é obrigatório pois o id pode ser nulo

            if (flight == null)
            {
                return NotFound();
            }

            flight.To = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(flight.To.Id);
            flight.From = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(flight.From.Id);

            return View(flight);
        }

        // GET: Flight/Edit/5       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetFlightWithObjectsAsync(id.Value);

            if (flight == null)
            {
                return NotFound();
            }

            CreateFlightViewModel model = new CreateFlightViewModel()
            {
                Id = flight.Id,
                Airplanes = _flightRepository.GetComboAirplanes(),
                Destinations = _flightRepository.GetComboDestinations(),
                Status = _flightRepository.GetComboStatus(),
                Tickets = _flightRepository.GetComboTickets(flight.Id),
                From = flight.From.Id,
                To = flight.To.Id,
                Departure = flight.Departure,
                Arrival = flight.Arrival,
                AirplaneId = flight.Airplane.Id,
                StatusId = flight.Status.Id,
            };

            if (model.StatusId == 2 || model.StatusId == 3) // Voos terminados ou cancelados não podem ser editados
            {
                // Obter o state active
                var status = _context.Status.Where(x => x.Id == model.StatusId).FirstOrDefault();
                ViewBag.message = $"Flight is {status.StatusName}! Can't be deleted";
                return View(model);
            }

            ViewBag.minDate = DateTime.Now;
            ViewBag.format = "dd/MM/yyyy HH:mm";
            return View(model);
        }




        // POST: Flight/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateFlightViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Saber se o estado mudou (só se pode alterar de activo para canceled)
                var newStatus = _context.Status.Where(x => x.Id == model.StatusId).FirstOrDefault();
                if (newStatus == null)
                {
                    return NotFound();
                }

                // Saber se o avião mudou
                var airplane = await _airplaneRepository.GetAirplaneWithUserAsync(model.AirplaneId);
                if (airplane == null)
                {
                    return NotFound();
                }
                bool isAirplaneChange = airplane.Id == model.AirplaneId ? false : true;

                bool isAvailable = true;

                if (isAirplaneChange) // Se o avião mudou tenho que verificar a disponibilidade do novo avião
                {
                    isAvailable = _flightRepository.AirplaneIsAvailable(model.AirplaneId, model.Departure, model.Arrival);
                }

                var to = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(model.To);
                var from = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(model.From);

                if (isAvailable)
                {
                    Flight flight = new Flight()
                    {
                        Id= model.Id,
                        From = from,
                        To = to,
                        Departure = model.Departure,
                        Arrival = model.Arrival,
                        Airplane = airplane,
                        Status = newStatus,
                        Business = airplane.BusinessSeats,
                        Economic = airplane.EconomySeats,
                    };

                    try
                    {
                        await _flightRepository.UpDateAsync(flight);


                        List<Ticket> ticketList = new List<Ticket>();

                        if (flight.Tickets != null)
                        {
                            // Depois de Fazer o update, enviar um email para todos os utilizadores com bilhetes, com os novos dados
                            ticketList = flight.Tickets.ToList();
                        }
                                             

                        if (ticketList.Count !=0)
                        {
                        
                            if (flight.Status.StatusName=="Active")
                            {
                                foreach (var item in ticketList)
                                {
                                    _mailHelper.SendMail(item.User.Email, "Flight changes", $"<h1>Flight changes</h1></br></br>" +
                                    $"Please consider the new flight details:</br>" +
                                    $"From: {item.Flight.From.City.Name} </br>" +
                                    $"To: {item.Flight.To.City.Name} </br>" +
                                    $"Departure: {item.Flight.Departure} </br>" +
                                    $"Arrival: {item.Flight.Arrival} </br>" +
                                    "Thank you for your attention");
                                }
                            }

                            else if (flight.Status.StatusName == "Canceled")
                            {

                                foreach (var item in ticketList)
                                {
                                    _mailHelper.SendMail(item.User.Email, "Flight canceled", $"<h1>Flight canceled</h1></br></br>" +
                                    $"Your flight:</br>" +
                                    $"From: {item.Flight.From.City.Name} </br>" +
                                    $"To: {item.Flight.To.City.Name} </br>" +
                                    $"Was canceled! Please, contact our customer service)");
                                }
                            }                           
                        }

                        return RedirectToAction(nameof(Index));
                    }

                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                        GetCombos(model);
                        ViewBag.minDate = DateTime.Now;
                        ViewBag.format = "dd/MM/yyyy HH:mm";
                        return View(model);
                    }
                }

                ViewBag.minDate = DateTime.Now;
                ViewBag.format = "dd/MM/yyyy HH:mm";
                GetCombos(model);
                ModelState.AddModelError(string.Empty, "Airplane isn't available. Choose another!");
                return View(model);
            }


            GetCombos(model);
            ViewBag.minDate = DateTime.Now;
            ViewBag.format = "dd/MM/yyyy HH:mm";
            return View(model);
        }

        // GET: Flight/Delete/5   
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetFlightWithObjectsAsync(id.Value);

            if (flight == null)
            {
                return NotFound();
            }

            var to = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(flight.To.Id);
            var from = await _destinationRepository.GetDestinationWithUserCityAndCoutryAsync(flight.From.Id);

            flight.From = from;
            flight.To = to;

            return View(flight);
        }



        // POST: Flight/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetByIdAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            try
            {
                await _flightRepository.DeleteAsync(flight); // Método já grava as alterações realizadas

                return RedirectToAction(nameof(Index));
            }

            catch (Exception) // Erro por algum motivo (bilhetes associados ao voo, por exemplo)
            {
                ViewBag.Message = "Flight with associated tickets. It can't be deleted!";

                return View();

            }
        }

        // GET: Tickets/ 
        public async Task<IActionResult> Tickets(int? id) // FlightId
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetFlightWithObjectsAsync(id.Value);

            if (flight == null)
            {
                return NotFound();
            }

            var tickets = _flightRepository.GetTickets(id.Value);

            ViewBag.flightId = flight.Id;
            return View(tickets);
        }

        public IActionResult booking()
        {
            return View();
        }


            // GET: Tickets/Delete/5   
            public async Task<IActionResult> CreateTicket(int? id) // FlightId
        {
            if (id == null)
            {
                return NotFound();
            }
            
            
            // Para criar um bilhete tenho que escolher o voo --> Já vai para a view
            // Criar um botão para ir à página de utilizadores escolher o user
            // Colocar uma drop down list para seleccionar a classe
            // colocar uma drop down para escolher os liugares disponiveis

            // Obter o voo
            var flight = await _flightRepository.GetFlightWithObjectsAsync(id.Value);

            //Obter os bilhetes do voo



            if (flight == null)
            {
                return NotFound();
            }

            var tickets = _flightRepository.GetTickets(id.Value);

            return View(tickets);
        }

    }
}
