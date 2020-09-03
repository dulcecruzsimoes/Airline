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

namespace Airline.Web.Controllers
{
    public class AirplainesController : Controller
    {
        private readonly IAirplaineRepository _repository;

        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public AirplainesController(IAirplaineRepository repository, IUserHelper userHelper, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _repository = repository;

            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }



        // GET: Airplaines
        public IActionResult Index()
        {
            return View( _repository.GetAll());
        }



        // GET: Airplaines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                
                return new NotFoundViewResult("MyNotFound");
            }

            var airplaine = await _repository.GetByIdAsync(id.Value);

            if (airplaine == null)
            {
             
                return new NotFoundViewResult("MyNotFound");
            }

            
            
            return View(airplaine);
        }



        // GET: Airplaines/Create
        public IActionResult Create()
        {
            return View();
        }



        // POST: Airplaines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AirplaineViewModel airplaineViewModel)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (airplaineViewModel.ImageFile != null)
                {
                   path = await _imageHelper.UpLoadImageAsync(airplaineViewModel.ImageFile, "Airplaines");              
                        
                }

                var airplaine = _converterHelper.ToAirplaine(airplaineViewModel, path, true);

                airplaine.User = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                await _repository.CreateAsync(airplaine);
                return RedirectToAction(nameof(Index));
            }
            return View(airplaineViewModel);
        }

       /* private Airplaine ToAirplaine(AirplaineViewModel model, string path)
        {
            return new Airplaine
            {
                Id = model.Id,
                Brand = model.Brand,
                Model = model.Model,
                ImageUrl = path,
                EconomySeats = model.EconomySeats,
                BusinessSeats = model.BusinessSeats,
                User = model.User
            };
        }*/

        // GET: Airplaines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                
                return new NotFoundViewResult("MyNotFound");
            }

            var airplaine = await _repository.GetByIdAsync(id.Value);

            if (airplaine == null)
            {
                
                return new NotFoundViewResult("MyNotFound");
            }

            var view = _converterHelper.ToAirplaineViewModel(airplaine);

            return View(view);
        }

        /*private AirplaineViewModel ToAirplaineViewModel(Airplaine airplaine)
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
        }*/




        // POST: Airplaines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AirplaineViewModel airplaineViewModel)
        {
         

            if (ModelState.IsValid)
            {
                try
                {
                    var path = string.Empty;

                    if (airplaineViewModel.ImageFile != null && airplaineViewModel.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UpLoadImageAsync(airplaineViewModel.ImageFile, "Airplaines");

                    }

                    var airplaine = _converterHelper.ToAirplaine(airplaineViewModel, path, false);
                    // TODO: change for logged user
                    airplaine.User = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                    await _repository.UpDateAsync(airplaine); // Método Update já grava as alterações
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _repository.ExistsAsync(airplaineViewModel.Id))
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
            return View(airplaineViewModel);
        }




        // GET: Airplaines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                
               return new NotFoundViewResult("MyNotFound");
            }

            var airplaine = await _repository.GetByIdAsync(id.Value);

            if (airplaine == null)
            {
               
                return new NotFoundViewResult("MyNotFound");
            }
            

            return View(airplaine);
        }



        // POST: Airplaines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airplaine = await _repository.GetByIdAsync(id);

            await _repository.DeleteAsync(airplaine); // Método já grava as alterações realizadas

            return RedirectToAction(nameof(Index));
        }


        public IActionResult MyNotFound() 
        {
            return View();
        
        }
        
    }
}
