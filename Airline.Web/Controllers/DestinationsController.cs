using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Airline.Web.Data;
using Airline.Web.Data.Entities;
using Airline.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Models;

namespace Airline.Web.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class DestinationsController : Controller
    {
        private readonly IDestinationRepository _repository;
        private readonly IUserHelper _userHelper;
        private readonly ICountryRepository _countryRepository;

        public DestinationsController(IDestinationRepository repository, IUserHelper userHelper, ICountryRepository countryRepository)
        {
            _repository = repository;
            _userHelper = userHelper;
            _countryRepository = countryRepository;
        }



        // GET: Destinations
        public IActionResult Index()
        {

            var destinations = _repository.GetAllWithUsersAndCountryAndCity(); 
            

            List<DestinationViewModel> lista = new List<DestinationViewModel>();

            foreach (Destination item in destinations)
            {

                lista.Add(new DestinationViewModel
                {
                    Id = item.Id,
                    Airport = item.Airport,
                    IATA = item.IATA,
                    UserId = item.User.Id,
                    City = item.City.Name,
                    Country = item.Country.Name,
                    Countries = _countryRepository.GetComboCountries(),
                    Cities = _countryRepository.GetComboCities(item.Country.Id),
                });

            }

            return View(lista);

        }





        // GET: Destinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _repository.GetDestinationWithUserCityAndCoutryAsync(id.Value); // .Value é obrigatório pois o id pode ser nulo

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }



        // GET: Destinations/Create
        public IActionResult Create()
        {
            var model = new DestinationViewModel();

            GetCombos(model);

            return View(model);
        }



        public void GetCombos(DestinationViewModel model)
        {
            model.Cities = _countryRepository.GetComboCities(0);
            model.Countries = _countryRepository.GetComboCountries();
           
        }

        // POST: Destinations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DestinationViewModel model)
        {
            if (ModelState.IsValid)
            {
                City city = await _countryRepository.GetCityAsync(model.CityId);

                Country country =  _countryRepository.GetCountryAsync(city);

                User user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                Destination destination = new Destination()
                {
                    Airport = model.Airport,
                    IATA = model.IATA,
                    City = city,
                    Country = country,
                    User = user,
                };

                try
                {
                    await _repository.CreateAsync(destination); // Ao usar o create grava logo

                    return RedirectToAction(nameof(Index)); //Redirecionamento para a página
                                                            // nameof(Index) é o mesmo que colocar ("Index")
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        
                        model.CityId = 0;
                        model.CountryId = 0;
                        model.Cities = _countryRepository.GetComboCities(0);
                        model.Countries = _countryRepository.GetComboCountries();
                        ModelState.AddModelError(string.Empty, "Already exists a destination with that IATA");
                        return View(model);
                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                        model.Cities = _countryRepository.GetComboCities(0);
                        model.Countries = _countryRepository.GetComboCountries();
                        return View(model);
                    }
                }               
            }
            return View(model);
        }



        // GET: Destinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

      
            var destination = await _repository.GetDestinationWithUserCityAndCoutryAsync(id.Value);

            if (destination == null)
            {
                return NotFound();
            }

            DestinationViewModel model = new DestinationViewModel()
            {
                Id = destination.Id,
                Airport = destination.Airport,
                IATA = destination.IATA,
                UserId = destination.User.Id,
                City = destination.City.Name,
                Country = destination.Country.Name,
                Countries = _countryRepository.GetComboCountries(),
                Cities = _countryRepository.GetComboCities(0)
            };

            
            return View(model);
        }



        public async Task<JsonResult> GetCitiesAsync(int? countryId)
        {
            if (countryId == 0)
            {
                Country country1 = new Country() { Id = 0, };
                return this.Json(country1);
            }

            var country = await _countryRepository.GetCountryWithCitiesAsync(countryId.Value);
            return this.Json(country.Cities.OrderBy(c => c.Name));

        }

        // POST: Destinations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DestinationViewModel model)
        {
        

            if (ModelState.IsValid)
            {
                try
                {
                    var city = await _countryRepository.GetCityAsync(model.CityId);

                    var country = await _countryRepository.GetByIdAsync(model.CountryId);

                    var user = await _userHelper.GetUserByIdAsync(model.UserId);

                    Destination destination = new Destination()
                    {
                        Id = model.Id,
                        IATA = model.IATA,
                        Airport = model.Airport,
                        City = city,
                        Country = country,
                        User = user
                    };

                    try
                    {
                        await _repository.UpDateAsync(destination); // O método Update já grava as alterações
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException.Message.Contains("duplicate"))
                        {
                            GetCombos(model);
                            model.CountryId = country.Id;
                            model.CityId = city.Id;
                            ModelState.AddModelError(string.Empty, "Already exists a destination with that IATA");
                            return View(model);
                        }

                        else
                        {
                   
                            model.CountryId = country.Id;
                            model.CityId = city.Id;
                            GetCombos(model);
                            ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                            return View(model);
                        }
                    }
                      
                    
                    
                    
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (! await _repository.ExistsAsync(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return View(model);
                    }
                }              
            }
            GetCombos(model);
            return View(model);
        }



        // GET: Destinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _repository.GetDestinationWithUserCityAndCoutryAsync(id.Value);            

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }



        // POST: Destinations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var destination = await _repository.GetByIdAsync(id);

            if (destination == null)
            {
                return NotFound();
            }

            try
            {
                await _repository.DeleteAsync(destination); // Método já grava as alterações realizadas

                return RedirectToAction(nameof(Index));
            }

            catch (Exception) // Erro por algum motivo
            {
                ViewBag.Message = "Destino utilizado em outros registos, não é possível apagar!";

                return View();

            }
        }

       
    }
}
