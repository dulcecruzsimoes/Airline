using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data;
using Airline.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Operators;

namespace Airline.Web.Controllers
{
    [Authorize (Roles = "Admin")] // Apenas o administrador tem acesso aos Roles

    public class RolesController : Controller
    {
        private readonly IUserHelper _userHelper;
        

        public RolesController(IUserHelper userHelper )
        {
            _userHelper = userHelper;
            
        }



        // GET: Roles
        public IActionResult Index()
        {
            return View(_userHelper.GetAll().OrderBy(x => x.Name));
        }


        
        // GET: Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _userHelper.GetRoleAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }


        
        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        

        // POST: Role/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(role.Name))
                {

                    ModelState.AddModelError(string.Empty, "Role name is required");
                    return View(role);                                       
                }

                

                if (await _userHelper.CheckRoleAsyn(role.Name)) // Existe o role
                {
                    ModelState.AddModelError(string.Empty, "Already exists a role with that name");
                    return View(role);

                }

                await _userHelper.CreateRoleAsyn(role.Name);
                              
                return RedirectToAction(nameof(Index));          
 
            }

            return View(role);
        }

    

        // GET: Role/Edit/5        
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _userHelper.GetRoleAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }



        // POST: Airplaines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(role.Name))
                {
                    ModelState.AddModelError(string.Empty, "Role name is required");
                    return View(role);
                }

                string name = role.Name;

                var result = await _userHelper.GetRoleByNameAsync(name);

                if (result != null && (result.Id != role.Id)) // Verificar se existe algum role com o nome pretendido e que não tenha o mesmo id. Posso qerer alterar para o mesmo...
                {
                    ModelState.AddModelError(string.Empty, "Already exists a role with that name");
                    return View(role);

                }

              

                var role2 = await _userHelper.GetRoleAsync(role.Id);

                role2.Name = role.Name;

                await _userHelper.UpdateRoleAsync(role2);

                return RedirectToAction(nameof(Index));

            }

            return View(role);          
        
        }



        
        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {

                return NotFound();
            }

            var role = await _userHelper.GetRoleAsync(id);

            if (role == null) // Se o Role não existe
            {
                return NotFound();
            }


            return View(role);
        }

        

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            if (id == null)
            {

                return NotFound();
            }

            var role = await _userHelper.GetRoleAsync(id);

            if (role == null) // Se o Role não existe
            {
                return NotFound();
            }

            await _userHelper.DeleteRoleAsync(role);            

            return RedirectToAction(nameof(Index));
        }


      
    }
}
