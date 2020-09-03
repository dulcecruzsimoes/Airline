using Airline.Web.Data.Entities;
using Airline.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
	public interface ICountryRepository : IGenericRepository<Country>
	{
		IQueryable GetCountriesWithCities();


		Task<Country> GetCountryWithCitiesAsync(int id);


		Task<City> GetCityAsync(int id);


		Task AddCityAsync(CityViewModel model);


		Task<int> UpdateCityAsync(City city);


		Task<int> DeleteCityAsync(City city);


		IEnumerable<SelectListItem> GetComboCountries();


		IEnumerable<SelectListItem> GetComboCities(int conuntryId);

		Country GetCountryAsync(City city);

		
	}
}

