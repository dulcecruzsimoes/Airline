using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Entities
{
    public class Country : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string Name { get; set; }


        public ICollection<City> Cities { get; set; }


        [Display(Name = "# Cities")]
        public int NumberCities { get { return this.Cities == null ? 0 : this.Cities.Count; } }

    }
}
