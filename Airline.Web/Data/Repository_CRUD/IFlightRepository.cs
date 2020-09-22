using Airline.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
    public interface IFlightRepository: IGenericRepository<Flight>
    {
        IEnumerable<SelectListItem> GetComboStatus();

        IEnumerable<Flight> GetFlightsByStatus(int statusId);

        IEnumerable<SelectListItem> GetComboDestinations();

        IEnumerable<SelectListItem> GetComboAirplanes();

        IEnumerable<SelectListItem> GetComboTickets(int id);

        bool AirplaneIsAvailable(int id, DateTime departure, DateTime arrival);

        List<Flight> GetAllWithObjects();

        Task<Flight> GetFlightWithObjectsAsync(int id);

        void UpdateFlightStatus(DateTime date);

        List<Ticket> GetTickets(int flightId);
    }
}
