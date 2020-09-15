using Airline.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
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


        public IEnumerable<SelectListItem> GetComboStates()
         {

             var list = _context.States
                 .Select(p => new SelectListItem
                 {
                     Text = p.StateName.ToString(),
                     Value = p.Id.ToString()
                 }).ToList();


             list.Insert(0, new SelectListItem
             {
                 Text = "(Select a state...)",
                 Value = "0"
             });

             return list;
        }

        public IEnumerable<Flight> GetFlightsByState(int stateId) 
        {
            return  _context.Flights.Where(x=>x.State.Id == stateId).ToList(); 

        
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

        public IEnumerable<SelectListItem> GetComboAirplaines()
        {
        
            // Obter a lista de todos os aviões
            var list = _context.Airplaines            
                     .Select(p => new SelectListItem
                     {
                         Text = p.Brand.ToString(),
                         Value = p.Id.ToString()
                     }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a airplaine...)",
                Value = "0"
            });
            return list;


        }

    }
}
