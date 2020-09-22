using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Airline.Web.Data;
using Airline.Web.Data.Entities;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Helpers;
using Airline.Web.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace Airline.Web.Controllers
{
    [Authorize(Roles = "Admin,Employee")] // Autorizado apenas para o Administrador e Empregado

    public class AirplanesController : Controller
    {
        private readonly IAirplaneRepository _repository;

        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public AirplanesController(IAirplaneRepository repository, IUserHelper userHelper, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _repository = repository;

            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }



        // GET: Airplanes
        public IActionResult Index()
        {
            return View( _repository.GetAll());
        }



        // GET: Airplanes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {

                return NotFound();
            }

            var airplane = await _repository.GetByIdAsync(id.Value);

            if (airplane == null)
            {
             
                return NotFound();
            }
            
            
            return View(airplane);
        }



        // GET: Airplanes/Create
        public IActionResult Create()
        {
            return View();
        }



        // POST: Airplaines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AirplaneViewModel airplaneViewModel)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (airplaneViewModel.ImageFile != null)
                {
                   path = await _imageHelper.UpLoadImageAsync(airplaneViewModel.ImageFile, "Airplanes");              
                        
                }

                var airplane = _converterHelper.ToAirplane(airplaneViewModel, path, true);

                airplane.User = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                await _repository.CreateAsync(airplane);

                return RedirectToAction(nameof(Index));
            }

            return View(airplaneViewModel);
        }



        // GET: Airplanes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                
                return new NotFoundViewResult("MyNotFound");
            }

            var airplane = await _repository.GetByIdAsync(id.Value);

            if (airplane == null)
            {
                
                return new NotFoundViewResult("MyNotFound");
            }

            var view = _converterHelper.ToAirplaneViewModel(airplane);

            return View(view);
        }




        // POST: Airplanes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AirplaneViewModel airplaneViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = string.Empty;

                    if (airplaneViewModel.ImageFile != null && airplaneViewModel.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UpLoadImageAsync(airplaneViewModel.ImageFile, "Airplaines");

                    }

                    var airplaine = _converterHelper.ToAirplane(airplaneViewModel, path, false);
                    // TODO: change for logged user
                    airplaine.User = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                    await _repository.UpDateAsync(airplaine); // Método Update já grava as alterações
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _repository.ExistsAsync(airplaneViewModel.Id))
                    {
                        return new NotFoundViewResult("MyNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(airplaneViewModel);
        }




        // GET: Airplanes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                
               return new NotFoundViewResult("MyNotFound");
            }

            var airplane = await _repository.GetByIdAsync(id.Value);

            if (airplane == null)
            {
               
                return new NotFoundViewResult("MyNotFound");
            }
            

            return View(airplane);
        }



        // POST: Airplanes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == 0)
            {
                return NotFound();

            }

            var airplane = await _repository.GetByIdAsync(id);

            if (airplane == null)
            {
                return NotFound();

            }

            try
            {
                await _repository.DeleteAsync(airplane); // Método já grava as alterações realizadas

                return RedirectToAction(nameof(Index));

            }
            catch (Exception) // Erro por algum motivo
            {
                ViewBag.Message = "Avião utilizado em outros registos, não é possível apagar!";

                return View();
                
            }
           
        }


        public IActionResult MyNotFound() 
        {
            return View();
        
        }
        
    }
}
