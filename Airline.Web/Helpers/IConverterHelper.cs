using Airline.Web.Data.Entities;
using Airline.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Airline.Web.Helpers
{
    public interface IConverterHelper
    {

        Airplane ToAirplane(AirplaneViewModel airplaneViewModel, string path, bool isNew);


        AirplaneViewModel ToAirplaneViewModel(Airplane airplane);

    }
}
