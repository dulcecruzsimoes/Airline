using Airline.Web.Data.Entities;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Helpers;
using Airline.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ICountryRepository _countryRepository;

        public ClientsController(IUserHelper userHelper, ICountryRepository countryRepository)
        {
            _userHelper = userHelper;
            _countryRepository = countryRepository;
        }

        public async Task<IActionResult> Index() 
        {            
            var users = await _userHelper.GetUsersInRoleAsync("Customer");
            return View(users);
        }


        public async Task<IActionResult> Details(string id)
        {
            var user = await _userHelper.GetUserByIdAsync(id);
            var model = new ChangeUserViewModel();

            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PhoneNumber = user.PhoneNumber;
                model.TaxNumber = user.TaxNumber;
                model.Username = user.UserName;
                model.SocialSecurityNumber = user.SocialSecurityNumber;


                var city = await _countryRepository.GetCityAsync(user.CityId);
                if (city != null)
                {
                    var country = _countryRepository.GetCountryAsync(city);
                    if (country != null)
                    {
                        model.CountryId = country.Id;
                        model.Cities = _countryRepository.GetComboCities(country.Id);
                        model.Countries = _countryRepository.GetComboCountries();
                        model.CityId = user.CityId;
                    }
                }

                return View(model);
            }

            return NotFound();
        }


        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userHelper.GetUserByIdAsync(id);
            var model = new ChangeUserViewModel();

            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PhoneNumber = user.PhoneNumber;
                model.TaxNumber = user.TaxNumber;
                model.Username = user.UserName;
                model.SocialSecurityNumber = user.SocialSecurityNumber;
                model.isActive = user.isActive;


                var city = await _countryRepository.GetCityAsync(user.CityId);
                if (city != null)
                {
                    var country = _countryRepository.GetCountryAsync(city);
                    if (country != null)
                    {
                        model.CountryId = country.Id;
                        model.Cities = _countryRepository.GetComboCities(country.Id);
                        model.Countries = _countryRepository.GetComboCountries();
                        model.CityId = user.CityId;
                    }
                }
                return View(model);
            }

            return NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> Edit(ChangeUserViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var city = await _countryRepository.GetCityAsync(model.CityId);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.TaxNumber = model.TaxNumber;
                    user.SocialSecurityNumber = model.SocialSecurityNumber;
                    user.CityId = model.CityId;
                    user.City = city;
                    user.UserName = model.Username;
                    user.isActive = model.isActive;

                    var result = await _userHelper.UpdateUserAsync(user);

                    if (result.Succeeded)
                    {
                        ViewBag.Message = "User updated!";
                        model.Cities = _countryRepository.GetComboCities(model.CountryId);
                        model.Countries = _countryRepository.GetComboCountries();
                        return this.View(model);
                    }
                    else
                    {

                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                        model.Cities = _countryRepository.GetComboCities(model.CountryId);
                        model.Countries = _countryRepository.GetComboCountries();
                        return this.View(model);

                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User no found.");
                    model.Cities = _countryRepository.GetComboCities(model.CountryId);
                    model.Countries = _countryRepository.GetComboCountries();
                    return this.View(model);
                }
            }

            model.Cities = _countryRepository.GetComboCities(model.CountryId);
            model.Countries = _countryRepository.GetComboCountries();
            return this.View(model);
        }


        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userHelper.GetUserByIdAsync(id);
         

            if (user != null)
            {
                return View(user);
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> Delete(User model)
        {
            var user = await _userHelper.GetUserByIdAsync(model.Id);
         

            if (user != null)
            {
                user.isActive = false;

                var result = await _userHelper.UpdateUserAsync(user);

                if (result.Succeeded)
                {
                    ViewBag.Message = "User deleted!";

                    return this.View(model);
                }
            }
            return NotFound();


        }
    }
}
