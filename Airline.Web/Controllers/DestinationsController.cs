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

namespace Airline.Web.Controllers
{
    [Authorize]
    public class DestinationsController : Controller
    {
        private readonly IDestinationRepository _repository;
        private readonly IUserHelper _userHelper;

        public DestinationsController(IDestinationRepository repository, IUserHelper userHelper)
        {
            _repository = repository;
            _userHelper = userHelper;
        }



        // GET: Destinations
        public IActionResult Index()
        {
            return View( _repository.GetAll().OrderBy(d=>d.Airport));
        }


        [Authorize(Roles = "Admin")]
        // GET: Destinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _repository.GetByIdAsync(id.Value); // .Value é obrigatório pois o id pode ser nulo

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }



        // GET: Destinations/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }



        // POST: Destinations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Destination destination)
        {
            if (ModelState.IsValid)
            {
                // TODO: change for logged user
                destination.User = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                await _repository.CreateAsync(destination); // Ao usar o create grava logo
                
                return RedirectToAction(nameof(Index)); //Redirecionamento para a página
                // nameof(Index) é o mesmo que colocar ("Index")
            }
            return View(destination);
        }



        // GET: Destinations/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _repository.GetByIdAsync(id.Value);

            if (destination == null)
            {
                return NotFound();
            }
            return View(destination);
        }



        // POST: Destinations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Destination destination)
        {
        

            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: change for logged user
                    destination.User = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                    await _repository.UpDateAsync(destination); // O método Update já grava as alterações

                    
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (! await _repository.ExistsAsync(destination.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(destination);
        }



        // GET: Destinations/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _repository.GetByIdAsync(id.Value);            

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }



        // POST: Destinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var destination = await _repository.GetByIdAsync(id);

            await _repository.DeleteAsync(destination); // Método delete grava logo as alterações            

            return RedirectToAction(nameof(Index));
        }

       
    }
}
