using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Web.Controllers
{
    public class MoviesController : Controller
    {


        public MoviesController()
        {
         
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dados()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Dados(MyViewModel model) 
        {
            if (ModelState.IsValid)
            {
                return View();
            }

            return View();
        
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            string title,
            Genre genre,
            DateTime releaseDate,
            string description,
            decimal price,
            bool preorder)
        {
            var modifiedReleaseDate = releaseDate;
            if (releaseDate == null)
            {
                modifiedReleaseDate = DateTime.Today;
            }

            #region snippet_TryValidateModel
            var movie = new Movie
            {
                Title = title,
                Genre = genre.ToString(),
                ReleaseDate = modifiedReleaseDate,
                Description = description,
                Price = price,
                Preorder = preorder,
            };

            TryValidateModel(movie);

            if (ModelState.IsValid)
            {
                

                return RedirectToAction(actionName: nameof(Index));
            }

            return View(movie);
            #endregion
        }
    }
}
