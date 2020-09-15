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
        IEnumerable<SelectListItem> GetComboStates();

        IEnumerable<Flight> GetFlightsByState(int stateId);

        IEnumerable<SelectListItem> GetComboDestinations();

        IEnumerable<SelectListItem> GetComboAirplaines();
    }
}
