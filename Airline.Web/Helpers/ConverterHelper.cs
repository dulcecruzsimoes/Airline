using Airline.Web.Data.Entities;
using Airline.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Airplane ToAirplane(AirplaneViewModel airplaneViewModel, string path, bool isNew)
        {
            return new Airplane
            {
                Id = isNew ? 0 : airplaneViewModel.Id, // Se o aviãofor for novo ainda não estiver na base de dados não lhe posso dar nenhum id
                Brand = airplaneViewModel.Brand,
                Model = airplaneViewModel.Model,
                ImageUrl = path,
                EconomySeats = airplaneViewModel.EconomySeats,
                BusinessSeats = airplaneViewModel.BusinessSeats,
                User = airplaneViewModel.User
            };
        }

        public AirplaneViewModel ToAirplaneViewModel(Airplane airplane)
        {
            return new AirplaneViewModel
            {
                Id = airplane.Id,
                Brand = airplane.Brand,
                Model = airplane.Model,
                BusinessSeats = airplane.BusinessSeats,
                EconomySeats = airplane.EconomySeats,
                User = airplane.User,
                ImageUrl = airplane.ImageUrl
            };
        }
    }
}
