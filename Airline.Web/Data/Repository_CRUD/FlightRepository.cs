using Airline.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
    public class FlightRepository: GenericRepository<Flight>, IFlightRepository
    {
        private DataContext _context { get; set; }


        public FlightRepository(DataContext context): base(context)
        {
            _context = context;
            // Colocar no construtor uma acção que me faça a actualização dos voos e que mude os estados para concluded
        }

        public async void UpdateFlightStatus(DateTime date) 
        {

           var list =  _context.Flights.Where(x => x.Arrival < date && x.Status.StatusName == "Active").ToList();

            var StateConclued =  _context.Status.Where(y => y.StatusName == "concluded").FirstOrDefault(); // Obter o estado

            foreach (var item in list)
            {
                Flight flight = item;
                flight.Status = StateConclued;
                _context.Flights.Update(flight);
                await _context.SaveChangesAsync();
            }
                
        
        }


        public async Task<Flight> GetFlightWithObjectsAsync(int id) 
        {
            return await _context.Flights
                .Include(d => d.Airplane)
                .Include(d => d.From)
                .Include(d => d.To)
                .Include(d => d.Status)
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();
        }

        public List<Flight> GetAllWithObjects()
        {
            return _context.Flights
                .Include(d => d.Airplane)
                .Include(d => d.From)
                .Include(d => d.To)
                .Include(d => d.Status)
                .ToList();
        }

        public IEnumerable<SelectListItem> GetComboStatus()
         {

             var list = _context.Status
                 .Select(p => new SelectListItem
                 {
                     Text = p.StatusName.ToString(),
                     Value = p.Id.ToString()
                 }).ToList();


             list.Insert(0, new SelectListItem
             {
                 Text = "(Select a status...)",
                 Value = "0"
             });

             return list;
        }

        public IEnumerable<SelectListItem> GetComboTickets(int id)
        {

            var list = _context.Tickets.Where(x => x.Id == id)
                .Select(p => new SelectListItem
                {
                    Text = p.User.FullName.ToString(),
                    Value = p.Id.ToString()
                })
                .ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Ticket...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<Flight> GetFlightsByStatus(int statusId) 
        {
            return  _context.Flights.Where(x=>x.Status.Id == statusId).ToList(); 

        
        }

        public IEnumerable<SelectListItem> GetComboDestinations()
        {

            var list = _context.Destinations
                     .Select(p => new SelectListItem
                     {
                         Text = p.City.Name.ToString(),
                         Value = p.Id.ToString()
                     }).ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "(Select a destination...)",
                Value = "0"
            });

            return list;


        }

        public IEnumerable<SelectListItem> GetComboAirplanes()
        {
        
            // Obter a lista de todos os aviões
            var list = _context.Airplanes            
                     .Select(p => new SelectListItem
                     {
                         Text = p.Brand.ToString(),
                         Value = p.Id.ToString()
                     }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a airplane...)",
                Value = "0"
            });
            return list;


        }

        public bool AirplaneIsAvailable(int id, DateTime departure, DateTime arrival) 
        {
            var flightList = _context.Flights.Where(voo => voo.Airplane.Id == id && voo.Status.Id==1).ToList(); // Obtenção de todos os voos para aquele avião

            if (flightList.Count != 0)
            {   
                // Tenho que ter sempre a comparação entre o par de datas

                // 1º Caso: Voo existente com data de partida menor que a data de partida Pretendida e data de chegada menor ou igual à data de chegada pretendida
                var case1 = flightList.Where(x => x.Arrival >= departure && x.Departure <= departure).Any();

                // 2º Caso: Voo existente com data de partida depois da data pretendida mas a data de partida é inferior à data de chegada
                var case2 = flightList.Where(x => x.Departure <= departure && x.Arrival >= arrival).Any();

                // 3º Caso: Nova data contida em datas de voo já existentes
                var case3 = flightList.Where(x => x.Departure <= departure && x.Arrival >= departure).Any();

                //ºCaso: Nova data a coincidir com data existente
                var case4 = flightList.Where(x => x.Departure == departure && x.Arrival == departure).Any();


                if (case1)
                {
                    return false;
                }

                else if (case2)
                {
                    return false;
                }

                else if (case3)
                {
                    return false;
                }


                else if (case4)
                {
                    return false;
                }
            }
           

            return true;
        
        }


        //==================TICKETS===============================

        public List<Ticket> GetTickets(int flightId)
        {

            List<Ticket> list = _context.Tickets
                                .Include(x => x.User)
                                .Include(x => x.Flight)
                                .Where(x => x.Flight.Id == flightId)
                                .ToList();
            return list;

        }

        public IEnumerable<SelectListItem> GetComboClasses()
        {
            var list = new List<SelectListItem>();


            list.Insert(0, new SelectListItem
            {
                Text = "(Select a class...)",
                Value = "0"
            });

            list.Insert(0, new SelectListItem
            {
                Text = "Economic",
                Value = "1"
            });

            list.Insert(0, new SelectListItem
            {
                Text = "Business",
                Value = "2"
            });

            return list;
        }


        public IEnumerable<SelectListItem> GetComboUsers()
        {
            // Obter a lista de todos os users
            var list = _context.Users
                     .Select(p => new SelectListItem
                     {
                         Text = p.Email,
                         Value = p.Id.ToString()
                     }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a client...)",
                Value = "0"
            });
            return list;
        }

    }
}
