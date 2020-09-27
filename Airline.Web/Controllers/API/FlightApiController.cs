using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airline.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightApiController : ControllerBase
    {

        private readonly DataContext _context;

        public FlightApiController(DataContext context)
        {
            _context = context;

        }

        [Produces("application/json")]
        [HttpGet("search")]
        public async Task<IActionResult> Search() 
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();

                // Fazer a pesquisa por aeroporto (cidade)
                var cityName =  _context.Destinations.Include(c=>c.City).
                                Where(p => p.City.Name.Contains(term))
                                               .Select(p => p.City.Name)                                    
                                               .ToList();         


                return Ok(cityName);

            } 
            catch (Exception)
            {

                return BadRequest();
            }
        
        
        
        }
    }
}
