using System.ComponentModel.DataAnnotations;

namespace Airline.Web.Data.Entities
{
    public class Ticket : IEntity
    {
        // Core já identifica como chave primária
        public int Id { get; set; }

        // Campo de preenchimento obrigatório
        [Required(ErrorMessage = "The field {0} is required")]
        public User User { get; set; }

        // Campo de preenchimento obrigatório
        [Required(ErrorMessage = "The field {0} is required")]
        public Flight Flight { get; set; }

        // Campo de preenchimento obrigatório
        [Required(ErrorMessage = "The field {0} is required")]
        public string Class { get; set; }

        // Campo de preenchimento obrigatório, o lugar vai corresponder ao lugar no avião
        [Required(ErrorMessage = "The field {0} is required")]
        public int Seat { get; set; }


    }
}
