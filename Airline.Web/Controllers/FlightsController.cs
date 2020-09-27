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
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public FlightsController(IUserHelper userHelper, IFlightRepository flightRepository, ITicketRepository ticketRepository, IAirplaneRepository airplaneRepository, 
            IDestinationRepository destinationRepository, DataContext context, IMailHelper mailHelper, IConfiguration configuration)
        {
            _userHelper = userHelper;
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
            _airplaneRepository = airplaneRepository;
            _destinationRepository = destinationRepository;
            _context = context;
            _mailHelper = mailHelper;
            _configuration = configuration;
      

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


                        List<Ticket> ticketList = _flightRepository.GetTickets(flight.Id);

                       
                            // Depois de Fazer o update, enviar um email para todos os utilizadores com bilhetes, com os novos dados
                                          

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

      


        // GET: Tickets/Create/5   
        public async Task<IActionResult> CreateTicket(int? id) // FlightId
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obter o voo
            var flight = await _flightRepository.GetFlightWithObjectsAsync(id.Value);

          
            if (flight == null)
            {
                return NotFound();
            }


            if (flight.Status.StatusName == "Canceled" || flight.Status.StatusName == "Concluded")
            {
                ViewBag.Message = "The flight isn't active! It´s impossible to create tickets!";
                return View();
            }
            //Obter os bilhetes do voo, se houverem voos disponiveis é encaminhado para a escolha do cliente

            var tickets = _flightRepository.GetTickets(id.Value);


            // Existem bilhetes disponíveis, escolher o cliente:

            TicketModelCreate model = new TicketModelCreate();

            List<User> users = _userHelper.GetAllUsers();

            model.Users = users;
            model.FlightId = flight.Id;

            return View(model);          
            
        }


        public async Task<IActionResult> Booking(TicketModelCreate model)
        {

                // 1º: Obter a lista das classes
                var list = _flightRepository.GetComboClasses(); // Obter as classes

                // 2º: Verificar a existência do user
                var user = await _userHelper.GetUserByEmailAsync(model.UserEmail);
                if (user== null)
                {
                    return this.RedirectToAction("Index", "Flights");
                }

                // 3º: Verificar a existência do voo
                var flight = _flightRepository.GetFlight(model.FlightId);
                if (flight == null)
                {
                return this.RedirectToAction("Index", "Flights");   
                }

                //4º: Obter a lista de bilhetes existentes para o voo
                var ticketsList = _ticketRepository.FlightTickets(model.FlightId);


                // 4º Criar a lista com as classes para passar para a view
                string[] TicketsClassArray = new string[64];

                List<int> TicketsClassList = new List<int>(); // 8 Executiva + 56 Económica = 64 lugares


             
                foreach (var item in ticketsList)
                {
                    int index = (item.Seat)-1;

                    TicketsClassArray[index] = "occupied";

                }
                        
                

                model.UserEmail = user.Email;
                model.FlightId = flight.Id;
                model.Classes = list;
                model.SeatIsAvailable = TicketsClassArray.ToList();
                return View(model);
        

        }

       

    
        public async Task<IActionResult> ShowTicket(TicketModelCreate model) 
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByEmailAsync(model.UserEmail);

                model.FullName = user.FullName;

                if (model.Class == 1)
                {
                    model.ClassName = "Economic";
                }

                else if (model.Class == 2)
                {
                    model.ClassName = "Business";

                }

                var flight = await _flightRepository.GetFlightWithObjectsAsync(model.FlightId);

                Destination fromDestination = await _destinationRepository.GetDestinationByIATAAsync(flight.From.IATA);
                Destination toDestination = await _destinationRepository.GetDestinationByIATAAsync(flight.To.IATA);
                model.From = fromDestination.City.Name;
                model.To = toDestination.City.Name;
                model.Date = flight.Departure.ToShortDateString();
                model.Time = flight.Departure.ToShortTimeString();
                return View(model);

            }

            return this.RedirectToAction("Index", "Flights");
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmTicket(TicketModelCreate model)
        {
            if (ModelState.IsValid)
            {
                Ticket ticket = new Ticket();

                User user = await _userHelper.GetUserByEmailAsync(model.UserEmail);

                Flight flight = _flightRepository.GetFlight(model.FlightId);

                ticket.Seat = model.Seat;
                ticket.User = user;
                ticket.Flight = flight;

                if (model.Class == 1)
                {
                    ticket.Class = "Economic";
                }

                if (model.Class == 2)
                {
                    ticket.Class = "Business";
                }

                try
                {
                    await _ticketRepository.CreateAsync(ticket);// Ao usar o create grava logo

                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                    // Criar um link que vai levar lá dentro uma acção. Quando o utilizador carregar neste link, 
                    // vai no controlador Account executar a action "ConfirmEmail"(Ainda será feita)
                    // Este ConfirmEmail vai receber um objecto novo que terá um userid e um token.
                    var tokenLink = this.Url.Action("Index", "Home", new
                    {
                        userid = user.Id,
                        token = myToken,

                    }, protocol: HttpContext.Request.Scheme);

                    _mailHelper.SendMail(model.UserEmail, "Ticket", $"<h1>Ticket Confirmation</h1>" +
                       $"Your ticket information, " +
                       $"Flight: {model.FlightId}, " +
                       $"Class: {ticket.Class}, " +
                       $"Date: {ticket.Seat}, " +
                       $"Click in this link to home page :</br></br><a href = \"{tokenLink}\">Airline</a>");


                    return RedirectToAction(nameof(Index)); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);                
                    return View(model);
                }

            }

            return this.RedirectToAction("Index", "Flights");
        }


    }
}
