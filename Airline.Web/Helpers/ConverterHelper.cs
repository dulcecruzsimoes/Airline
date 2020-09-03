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
        public Airplaine ToAirplaine(AirplaineViewModel airplaineViewModel, string path, bool isNew)
        {
            return new Airplaine
            {
                Id = isNew ? 0 : airplaineViewModel.Id, // Se o aviãofor for novo ainda não estiver na base de dados não lhe posso dar nenhum id
                Brand = airplaineViewModel.Brand,
                Model = airplaineViewModel.Model,
                ImageUrl = path,
                EconomySeats = airplaineViewModel.EconomySeats,
                BusinessSeats = airplaineViewModel.BusinessSeats,
                User = airplaineViewModel.User
            };
        }

        public AirplaineViewModel ToAirplaineViewModel(Airplaine airplaine)
        {
            return new AirplaineViewModel
            {
                Id = airplaine.Id,
                Brand = airplaine.Brand,
                Model = airplaine.Model,
                BusinessSeats = airplaine.BusinessSeats,
                EconomySeats = airplaine.EconomySeats,
                User = airplaine.User,
                ImageUrl = airplaine.ImageUrl
            };
        }
    }
}
