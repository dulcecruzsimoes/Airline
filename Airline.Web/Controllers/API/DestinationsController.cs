using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Web.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class DestinationsController : Controller
    {
        private readonly IDestinationRepository _repository;

        public DestinationsController(IDestinationRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]

        public IActionResult GetAllDestinations() 
        {

            return Ok(_repository.GetAllWithUsers());
        }
    }
}
