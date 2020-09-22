using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data.Entities;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Linq;

namespace Airline.Web.Controllers
{
    [Authorize(Roles = "Admin,Employee")] // Autorizado apenas para o Administrador e Empregado

    public class CountriesController : Controller
    {
        private readonly ICountryRepository _countryRepository;


        // Construtor
        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;

        }


        // Apagar Cidade
        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _countryRepository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            try
            {
                var countryId = await _countryRepository.DeleteCityAsync(city);
                return this.RedirectToAction($"Details/{countryId}");
            }
            catch (Exception)
            {
                ViewBag.Message = "City can't be deleted, is linked to a field in use";
                return View() ;
            }
            
        }


        // Editar Cidade
        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _countryRepository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }


        // Post do Editar Cidade
        [HttpPost]
        public async Task<IActionResult> EditCity(City city)
        {
            if (this.ModelState.IsValid)
            {
                var countryId = await _countryRepository.UpdateCityAsync(city);
                if (countryId != 0)
                {
                    return this.RedirectToAction($"Details/{countryId}");
                }
            }

            return this.View(city);
        }


        // Adicionar Cidade
        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            var model = new CityViewModel { CountryId = country.Id };
            return View(model);
        }


        // Post do adicionar Cidade
        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await _countryRepository.AddCityAsync(model);
                return this.RedirectToAction($"Details/{model.CountryId}");
            }

            return this.View(model);
        }


        // Index - Apresenta todos os países e as respectivas cidades
        public IActionResult Index()
        {
            var countries = _countryRepository.GetCountriesWithCities();

            List<CountryViewModel> lista = new List<CountryViewModel>();

            foreach (Country item in countries)
            {
                List<City> CitiesList = new List<City>();

                CitiesList = item.Cities.ToList();


                lista.Add(new CountryViewModel
                {
                    CountryId = item.Id,
                    Name = item.Name,
                    Cities = CitiesList                   
                }) ;
                

            }

            return View(lista);
        }

        // Detalhes do País - Apresenta o país com as respectivas cidades: permite adicionar cidades
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetCountryWithCitiesAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }


        // Criar um novo País
        public IActionResult Create()
        {
            return View();
        }


        // Post do Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _countryRepository.CreateAsync(country);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Already there is a country with that name");
                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }

                }
            }

            return View(country);
        }

        // Editar o País
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.UpDateAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }


        // Apagar o país
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetCountryWithCitiesAsync(id.Value);

            if (country == null)
            {
                return NotFound();
            }

            if (country.NumberCities!=0)
            {
         
                ViewBag.Message = "You need to delet the cities first!";

                return View();
            }

            await _countryRepository.DeleteAsync(country);
            return RedirectToAction(nameof(Index));
        }
    }
}
